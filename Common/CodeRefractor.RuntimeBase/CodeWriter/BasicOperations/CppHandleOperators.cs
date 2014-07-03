#region Usings

using System;
using System.IO;
using System.Text;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;
using CodeRefractor.RuntimeBase.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    internal static class CppHandleOperators
    {
        public static bool HandleAssignmentOperations(MidRepresentationVariables vars, StringBuilder bodySb,
            LocalOperation operation, OperationKind kind, TypeDescriptionTable typeTable, MethodInterpreter interpreter)
        {
            switch (kind)
            {
                case OperationKind.Assignment:
                    HandleAssign(bodySb, operation, vars, typeTable, interpreter);
                    break;
                case OperationKind.BinaryOperator:
                    HandleOperator(operation, bodySb);
                    break;
                case OperationKind.UnaryOperator:
                    HandleUnaryOperator((UnaryOperator) operation, bodySb);
                    break;
                case OperationKind.SetField:
                    HandleSetField(operation, bodySb);
                    break;
                case OperationKind.GetField:
                    HandleGetField(operation, bodySb, interpreter);
                    break;
                case OperationKind.SetStaticField:
                    HandleSetStaticField(operation, bodySb);
                    break;
                case OperationKind.GetStaticField:
                    HandleLoadStaticField(operation, bodySb);
                    break;
                case OperationKind.GetArrayItem:
                    HandleReadArrayItem(operation, bodySb, interpreter);
                    break;

                case OperationKind.SetArrayItem:
                    HandleSetArrayValue(operation, bodySb, interpreter);
                    break;
                case OperationKind.NewObject:
                    HandleNewObject(operation, bodySb, vars, typeTable, interpreter);
                    break;
                case OperationKind.NewArray:
                    HandleNewArray(operation, bodySb, interpreter);
                    break;
                case OperationKind.AddressOfArrayItem:
                    HandleGetAddressOfArrayItem(operation, bodySb);
                    break;
                case OperationKind.RefAssignment:
                    HandleRefAssignment(operation, bodySb);
                    break;
                case OperationKind.DerefAssignment:
                    HandleDerefAssignment(operation, bodySb);
                    break;

                case OperationKind.FieldRefAssignment:
                    HandleFieldRefAssignment(operation, bodySb);
                    break;
                case OperationKind.LoadFunction:
                    HandleLoadFunction(operation, bodySb);
                    break;
                case OperationKind.SizeOf:
                    HandleSizeOf(operation, bodySb);
                    break;
                default:
                    return false;
            }
            return true;
        }


        private static void HandleAssign(StringBuilder sb, LocalOperation operation, MidRepresentationVariables vars,
            TypeDescriptionTable typeTable, MethodInterpreter interpreter)
        {
            var assignment = (Assignment) operation;

            var assignedTo = assignment.AssignedTo;
            var localVariable = assignment.Right as LocalVariable;
            if (localVariable != null)
            {
                var leftVarType = assignment.AssignedTo.ComputedType();
                var rightVarType = assignment.Right.ComputedType();
                if (leftVarType != rightVarType)
                {
                    if (rightVarType.ClrType.IsPointer)
                    {
                        sb.AppendFormat("{0} = *{1};", assignedTo, localVariable.Name);
                        return;
                    }
                }
                var assignedToData = interpreter.AnalyzeProperties.GetVariableData(assignedTo);
                var localVariableData = interpreter.AnalyzeProperties.GetVariableData(localVariable);
                var rightVar = localVariable;
                var description = assignedTo.ComputedType();
                if (description.ClrTypeCode != TypeCode.Object || 
                    assignedToData == localVariableData)
                {
                    sb.AppendFormat("{0} = {1};", assignedTo.Name, rightVar.Name);
                    return;
                }
                switch (assignedToData)
                {
                    case EscapingMode.Pointer:
                        switch (localVariableData)
                        {
                            case EscapingMode.Stack:
                                sb.AppendFormat("{0} = &{1};", assignedTo.Name, rightVar.Name);
                                return;
                            case EscapingMode.Smart:
                                sb.AppendFormat("{0} = ({1}).get();", assignedTo.Name, rightVar.Name);
                                return;
                        }
                        break;

                    case EscapingMode.Smart:
                        throw new InvalidDataException("Case not possible!");
                }
                throw new InvalidDataException("Case not handled");
            }
            else
            {
                sb.AppendFormat("{0} = {1};", assignedTo.Name, assignment.Right.ComputedValue());
            }
        }

        private static void HandleGetAddressOfArrayItem(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (RefArrayItemAssignment) operation;
            bodySb.AppendFormat("{0} = & ({1}->Items[{2}]);", value.Left.Name, value.ArrayVar.Name, value.Index.Name);
        }

        private static void HandleLoadFunction(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (FunctionPointerStore) operation;
            var leftData = assign.AssignedTo;
            var info = assign.FunctionPointer;
            var methodName = info.ClangMethodSignature();
            bodySb.AppendFormat("{0}=&({1});", leftData.Name, methodName);
        }

        private static void HandleSizeOf(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (SizeOfAssignment) operation;
            var leftData = (IdentifierValue) assign.AssignedTo;
            var rightData = assign.Right.ToCppName();
            bodySb.AppendFormat("{0} = sizeof({1});", leftData.Name, rightData);
        }

        private static void HandleRefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (RefAssignment) operation;
            var leftData = (IdentifierValue) assign.Left;
            var rightData = (IdentifierValue) assign.Right;
            bodySb.AppendFormat("{0} = &{1};", leftData.Name, rightData.Name);
        }

        private static void HandleFieldRefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (FieldRefAssignment) operation;
            var leftData = assign.Left;
            var rightData = assign.Right;
            var fieldName = assign.Field.Name;
            bodySb.AppendFormat("{0} = &{1}->{2};", leftData.Name, rightData.Name, fieldName);
        }

        private static void HandleDerefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (DerefAssignment) operation;
            var leftData = (IdentifierValue) assign.Left;
            var rightData = (IdentifierValue) assign.Right;
            bodySb.AppendFormat("{0} = *{1};", leftData.Name, rightData.Name);
        }

        private static void HandleLoadStaticField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation;
            var rightData = (StaticFieldGetter) assign.Right;
            bodySb.AppendFormat("{0} = {1}::{2};", assign.AssignedTo.Name,
                rightData.DeclaringType.ClrType.ToCppMangling(),
                rightData.FieldName.ValidName());
        }

        private static void HandleSetStaticField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation;
            var rightData = (StaticFieldSetter) assign.AssignedTo;
            bodySb.AppendFormat("{1}::{2} = {0};", assign.Right.Name,
                rightData.DeclaringType.ToCppMangling(),
                rightData.FieldName.ValidName());
        }

        public static void HandleOperator(object operation, StringBuilder sb)
        {
            var instructionOperator = (OperatorBase) operation;
            var localOperator = instructionOperator;
            var binaryOperator = instructionOperator as BinaryOperator;
            var unaryOperator = instructionOperator as UnaryOperator;

            var operationName = localOperator.Name;
            switch (operationName)
            {
                case OpcodeOperatorNames.Add:
                    HandleAdd(binaryOperator, sb);
                    break;
                case OpcodeOperatorNames.Sub:
                    HandleSub(binaryOperator, sb);
                    break;
                case OpcodeOperatorNames.Mul:
                    HandleMul(binaryOperator, sb);
                    break;
                case OpcodeOperatorNames.Div:
                    HandleDiv(binaryOperator, sb);
                    break;
                case OpcodeOperatorNames.Rem:
                    HandleRem(binaryOperator, sb);
                    break;

                case OpcodeOperatorNames.Ceq:
                    HandleCeq(binaryOperator, sb);
                    break;

                case OpcodeOperatorNames.Cgt:
                    HandleCgt(binaryOperator, sb);
                    break;

                case OpcodeOperatorNames.Clt:
                    HandleClt(binaryOperator, sb);
                    break;

                case OpcodeOperatorNames.And:
                    HandleAnd(binaryOperator, sb);
                    break;
                case OpcodeOperatorNames.Or:
                    HandleOr(binaryOperator, sb);
                    break;
                case OpcodeOperatorNames.Xor:
                    HandleXor(binaryOperator, sb);
                    break;

                case OpcodeOperatorNames.Not:
                    HandleNot(unaryOperator, sb);
                    break;
                case OpcodeOperatorNames.Neg:
                    HandleNeg(unaryOperator, sb);
                    break;

                case OpcodeOperatorNames.LoadArrayRef:
                    HandleLoadArrayRef(binaryOperator, sb);
                    break;

                case OpcodeOperatorNames.LoadLen:
                    HandleLoadLen(unaryOperator, sb);
                    break;

                case OpcodeOperatorNames.ConvI4:
                    HandleConvI4(unaryOperator, sb);
                    break;

                case OpcodeOperatorNames.ConvI8:
                    HandleConvI8(unaryOperator, sb);
                    break;

                case OpcodeOperatorNames.ConvR8:
                    HandleConvR8(unaryOperator, sb);
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Operation '{0}' is not handled", operationName));
            }
        }

        public static void HandleUnaryOperator(UnaryOperator operation, StringBuilder sb)
        {
            var localVar = operation;

            var unaryOperator = localVar;

            var operationName = localVar.Name;
            switch (operationName)
            {
                case OpcodeOperatorNames.Not:
                    HandleNot(localVar, sb);
                    break;
                case OpcodeOperatorNames.Neg:
                    HandleNeg(localVar, sb);
                    break;

                case OpcodeOperatorNames.LoadLen:
                    HandleLoadLen(unaryOperator, sb);
                    break;

                case OpcodeOperatorNames.ConvI:
                    HandleConvI(unaryOperator, sb);
                    break;
                case OpcodeOperatorNames.ConvU1:
                    HandleConvU1(unaryOperator, sb);
                    break;
                case OpcodeOperatorNames.ConvI4:
                    HandleConvI4(unaryOperator, sb);
                    break;
                case OpcodeOperatorNames.ConvI8:
                    HandleConvI8(unaryOperator, sb);
                    break;

                case OpcodeOperatorNames.ConvR4:
                    HandleConvR4(unaryOperator, sb);
                    break;

                case OpcodeOperatorNames.ConvR8:
                    HandleConvR8(unaryOperator, sb);
                    break;

                default:
                    throw new InvalidOperationException(String.Format("Operation '{0}' is not handled", operationName));
            }
        }

        private static void HandleClt(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} < {2})?1:0;", local, left, right);
        }

        private static void HandleCgt(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} > {2})?1:0;", local, left, right);
        }

        private static void HandleCeq(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} == {2})?1:0;", local, left, right);
        }

        private static void HandleNeg(UnaryOperator localVar, StringBuilder sb)
        {
            var operat = localVar;
            sb.AppendFormat("{0} = -{1};", localVar.AssignedTo.Name, operat.Left.Name);
        }

        private static void HandleConvR4(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (float){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        private static void HandleConvR8(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (double){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        private static void HandleConvI(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (void*){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        private static void HandleConvU1(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (System_Byte){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        private static void HandleConvI4(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (int){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        private static void HandleConvI8(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (System_Int64){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        private static void HandleLoadLen(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = {1}->Length;", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        private static void HandleLoadArrayRef(BinaryOperator binaryOperator, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(binaryOperator, out right, out left, out local);

            sb.AppendFormat("{0}={1}[{2}];", local, right, left);
        }

        private static void HandleNot(UnaryOperator localVar, StringBuilder sb)
        {
            var local = localVar.AssignedTo.Name;
            string left;
            GetUnaryOperandNames(localVar, out left);
            sb.AppendFormat("{0} = !{1};", local, left);
        }

        private static void HandleXor(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}^{2};", local, left, right);
        }

        private static void HandleOr(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}|{2};", local, left, right);
        }

        private static void HandleAnd(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}&{2};", local, left, right);
        }

        private static void HandleMul(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}*{2};", local, left, right);
        }

        private static void HandleDiv(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}/{2};", local, left, right);
        }

        private static void HandleRem(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}%{2};", local, left, right);
        }

        private static void GetBinaryOperandNames(BinaryOperator localVar, out string right,
            out string left, out string local)
        {
            local = localVar.AssignedTo.Name;
            var leftVar = localVar.Left as LocalVariable;
            left = leftVar == null ? localVar.Left.ToString() : leftVar.Name;
            var rightVar = localVar.Right as LocalVariable;
            right = rightVar == null ? localVar.Right.ToString() : rightVar.Name;
        }

        private static void GetUnaryOperandNames(UnaryOperator localVar, out string left)
        {
            left = localVar.Left.Name;
        }

        private static void HandleSub(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}-{2};", local, left, right);
        }

        private static void HandleAdd(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);
            if (localVar.Right.ComputedType().ClrType == typeof (IntPtr))
            {
                sb.AppendFormat("{0} = {1}+(size_t){2};", local, left, right);

                return;
            }

            sb.AppendFormat("{0} = {1}+{2};", local, left, right);
        }


        private static void HandleSetArrayValue(LocalOperation operation, StringBuilder sb,
            MethodInterpreter interpreter)
        {
            var arrayItem = (SetArrayElement)operation;
            var variableData = interpreter.AnalyzeProperties.GetVariableData(arrayItem.Instance);
            switch (variableData)
            {
                case EscapingMode.Stack:
                    sb.AppendFormat("{0}[{1}] = {2}; ",
                        arrayItem.Instance.Name,
                        arrayItem.Index.Name,
                        arrayItem.Right.ComputedValue());
                    return;
                default:
                    sb.AppendFormat("(*{0})[{1}] = {2}; ",
                        arrayItem.Instance.Name,
                        arrayItem.Index.Name,
                        arrayItem.Right.ComputedValue());
                    return;
            }
        }

        private static void HandleReadArrayItem(LocalOperation operation, StringBuilder bodySb,
            MethodInterpreter interpreter)
        {
            var valueSrc = (GetArrayElement)operation;
            var parentType = valueSrc.Instance.ComputedType();
            var variableData = interpreter.AnalyzeProperties.GetVariableData(valueSrc.AssignedTo);
            switch (variableData)
            {
                case EscapingMode.Smart:
                    bodySb.AppendFormat((parentType.ClrType.IsClass || parentType.ClrType.IsInterface)
                        ? "{0} = (*{1})[{2}];"
                        : "{0} = {1}[{2}];",
                        valueSrc.AssignedTo.Name, valueSrc.Instance.Name, valueSrc.Index.Name);
                    return;
                case EscapingMode.Pointer:
                    bodySb.AppendFormat((parentType.ClrType.IsClass || parentType.ClrType.IsInterface)
                        ? "{0} = ((*{1})[{2}]).get();"
                        : "{0} = ({1}[{2}]).get();",
                        valueSrc.AssignedTo.Name, valueSrc.Instance.Name, valueSrc.Index.Name);

                    return;
            }
        }

        private static void HandleGetField(LocalOperation operation, StringBuilder bodySb,
            MethodInterpreter interpreter)
        {
            var fieldGetterInfo = (GetField) operation;
            var assignedFrom = fieldGetterInfo.Instance;
            var assignedFromData = interpreter.AnalyzeProperties.GetVariableData(assignedFrom);
            var isOnStack = assignedFromData == EscapingMode.Stack;
            var fieldText = String.Format(isOnStack ? "{0}.{1}" : "{0}->{1}", fieldGetterInfo.Instance.Name,
                fieldGetterInfo.FieldName.ValidName());

            var assignedTo = fieldGetterInfo.AssignedTo;
            var assignedToData = interpreter.AnalyzeProperties.GetVariableData(assignedTo);
            switch (assignedToData)
            {
                case EscapingMode.Smart:
                    bodySb.AppendFormat("{0} = {1};", assignedTo.Name, fieldText);
                    break;

                case EscapingMode.Pointer:
                    bodySb.AppendFormat("{0} = {1}.get();", assignedTo.Name, fieldText);
                    break;
            }
        }

        private static void HandleSetField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (SetField) operation;

            if (assign.Right is ConstValue)
            {
                if (assign.FixedType.ClrType == typeof (string))
                {
                    bodySb.AppendFormat("{0}->{1} = {2};", assign.Instance.Name,
                        assign.FieldName.ValidName(), assign.Right.ComputedValue());
                }
                else
                {
                      bodySb.AppendFormat("{0}->{1} = {2};", assign.Instance.Name,
                    assign.FieldName.ValidName(), assign.Right.Name);
                }
            }

            else
            {
                bodySb.AppendFormat("{0}->{1} = {2};", assign.Instance.Name,
                    assign.FieldName.ValidName(), assign.Right.Name);
            }
        }

        private static void HandleNewArray(LocalOperation operation, StringBuilder bodySb, MethodInterpreter interpreter)
        {
            var assignment = (NewArrayObject)operation;
            var arrayData = assignment;

            var assignedData = interpreter.AnalyzeProperties.GetVariableData(assignment.AssignedTo);
            switch (assignedData)
            {
                case EscapingMode.Stack:
                    bodySb.AppendFormat("Array <{1}> {0} ({2}); ",
                        assignment.AssignedTo.Name,
                        arrayData.TypeArray.ToCppName(),
                        arrayData.ArrayLength.Name);
                    break;
                default:
                    bodySb.AppendFormat("{0} = std::make_shared< Array <{1}> >({2}); ",
                        assignment.AssignedTo.Name,
                        arrayData.TypeArray.ToCppName(),
                        arrayData.ArrayLength.Name);
                    break;
            }
        }


        private static void HandleNewObject(LocalOperation operation, StringBuilder bodySb,
            MidRepresentationVariables vars,
            TypeDescriptionTable typeTable, MethodInterpreter interpreter)
        {
            var value = (NewConstructedObject)operation;
            var rightValue = value;
            var localValue = rightValue.Info;

            var declaringType = localValue.DeclaringType;
            var cppName = declaringType.ToDeclaredVariableType(true, EscapingMode.Stack);
            var assignedData = interpreter.AnalyzeProperties.GetVariableData(value.AssignedTo);
            var isStack = assignedData == EscapingMode.Stack;
            if (isStack)
            {
                bodySb
                    .AppendFormat("{1} {0};", value.AssignedTo.Name, cppName);
            }
            else
            {
                bodySb.AppendFormat("{0} = std::make_shared<{1}>();", value.AssignedTo.Name, cppName);
            }
            bodySb.AppendLine();
            typeTable.SetIdOfInstance(bodySb, value.AssignedTo, declaringType, isStack);
        }
    }
}