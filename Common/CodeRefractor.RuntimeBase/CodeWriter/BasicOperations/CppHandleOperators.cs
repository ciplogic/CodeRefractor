﻿#region Uses

using System;
using System.IO;
using System.Text;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.Shared;
using CodeRefractor.TypeInfoWriter;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.CodeWriter.BasicOperations
{
    static class CppHandleOperators
    {
        public static bool HandleAssignmentOperations(StringBuilder bodySb, LocalOperation operation, OperationKind kind,
            TypeDescriptionTable typeTable, MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            switch (kind)
            {
                case OperationKind.Assignment:
                    HandleAssign(bodySb, operation, interpreter, crRuntime);
                    break;
                case OperationKind.BinaryOperator:
                    HandleOperator(operation, bodySb, crRuntime);
                    break;
                case OperationKind.UnaryOperator:
                    HandleUnaryOperator((UnaryOperator) operation, bodySb);
                    break;
                case OperationKind.SetField:
                    HandleSetField(operation, bodySb, crRuntime);
                    break;
                case OperationKind.GetField:
                    HandleGetField(operation, bodySb, interpreter);
                    break;
                case OperationKind.SetStaticField:
                    HandleSetStaticField(operation, bodySb);
                    break;
                case OperationKind.GetStaticField:
                    HandleLoadStaticField(operation, bodySb, crRuntime);
                    break;
                case OperationKind.GetArrayItem:
                    HandleReadArrayItem(operation, bodySb, interpreter, crRuntime);
                    break;

                case OperationKind.SetArrayItem:
                    HandleSetArrayValue(operation, bodySb, interpreter);
                    break;
                case OperationKind.NewObject:
                    HandleNewObject(operation, bodySb, typeTable, interpreter, crRuntime);
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
                    HandleLoadFunction(operation, bodySb, crRuntime);
                    break;
                case OperationKind.SizeOf:
                    HandleSizeOf(operation, bodySb);
                    break;
                default:
                    return false;
            }
            return true;
        }

        private static void HandleAssign(StringBuilder sb, LocalOperation operation, MethodInterpreter interpreter,
            ClosureEntities closureEntities)
        {
            var assignment = (Assignment)operation;

            var assignedTo = assignment.AssignedTo;


            var localVariable = assignment.Right as LocalVariable;

            var leftVarType = assignment.AssignedTo.ComputedType().GetClrType(closureEntities);
            var rightVarType = assignment.Right.ComputedType().GetClrType(closureEntities);

            var isderef = false;
            if (localVariable == null && (assignment.Right as ConstValue) != null &&
                ((ConstValue)assignment.Right).Value is LocalVariable)
            {
                isderef = true;
                localVariable = (LocalVariable)((ConstValue)assignment.Right).Value;
            }

            if (localVariable != null)
            {
                if (leftVarType != rightVarType)
                {
                    if (rightVarType.IsPointer || isderef)
                    {
                        sb.AppendFormat("{0} = *{1};", assignedTo.VarName, localVariable.Name);
                        return;
                    }
                    //Handke byrefs
                    if (leftVarType.IsByRef && !rightVarType.IsByRef)
                    {
                        sb.AppendFormat("*{0} = {1};", assignedTo.Name, assignment.Right.ComputedValue());
                        return;
                    }
                    //                    if (leftVarType.IsByRef)
                    //                    {
                    //                        sb.AppendFormat("{0} = &{1};", assignedTo.VarName, localVariable.Name);
                    //                        return;
                    //                    }
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
            //Handke byrefs
            if (leftVarType.IsByRef && !rightVarType.IsByRef)
            {
                sb.AppendFormat("*{0} = {1};", assignedTo.Name, assignment.Right.ComputedValue());
            }
            else
            {
                sb.AppendFormat("{0} = {1};", assignedTo.Name, assignment.Right.ComputedValue());
            }
        }

        static void HandleGetAddressOfArrayItem(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (RefArrayItemAssignment)operation;
            bodySb.AppendFormat("{0} = & ({1}->Items[{2}]);", value.Left.Name, value.ArrayVar.Name, value.Index.Name);
        }

        static void HandleLoadFunction(LocalOperation operation, StringBuilder bodySb, ClosureEntities crRuntime)
        {
            var assign = (FunctionPointerStore)operation;
            var leftData = assign.AssignedTo;
            var info = assign.FunctionPointer;
            var methodName = info.ClangMethodSignature(crRuntime);
            bodySb.AppendFormat("{0}=&({1});", leftData.Name, methodName);
        }

        static void HandleSizeOf(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (SizeOfAssignment)operation;
            var leftData = (IdentifierValue)assign.AssignedTo;
            var rightData = assign.Right.ToCppName();
            bodySb.AppendFormat("{0} = sizeof({1});", leftData.Name, rightData);
        }

        static void HandleRefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (RefAssignment)operation;
            var leftData = (IdentifierValue)assign.Left;
            var rightData = (IdentifierValue)assign.Right;
            bodySb.AppendFormat("{0} = &{1};", leftData.Name, rightData.Name);
        }

        static void HandleFieldRefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (FieldRefAssignment)operation;
            var leftData = assign.Left;
            var rightData = assign.Right;
            var fieldName = assign.Field.Name;
            bodySb.AppendFormat("{0} = &({1}->{2});", leftData.Name, rightData.Name, fieldName);
        }

        static void HandleDerefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (DerefAssignment)operation;
            var leftData = (IdentifierValue)assign.Left;
            var rightData = (IdentifierValue)assign.Right;
            bodySb.AppendFormat("{0} = *{1};", leftData.Name, rightData.Name);
        }

        static void HandleLoadStaticField(LocalOperation operation, StringBuilder bodySb,
            ClosureEntities closureEntities)
        {
            var assign = (Assignment)operation;
            var rightData = (StaticFieldGetter)assign.Right;
            bodySb.AppendFormat("{0} = {1}::{2};", assign.AssignedTo.Name,
                rightData.DeclaringType.GetClrType(closureEntities).ToCppMangling(),
                rightData.FieldName.ValidName());
        }

        static void HandleSetStaticField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment)operation;
            var rightData = (StaticFieldSetter)assign.AssignedTo;
            bodySb.AppendFormat("{1}::{2} = {0};", assign.Right.Name,
                rightData.DeclaringType.ToCppMangling(),
                rightData.FieldName.ValidName());
        }

        public static void HandleOperator(object operation, StringBuilder sb, ClosureEntities closureEntities)
        {
            var instructionOperator = (OperatorBase) operation;
            var localOperator = instructionOperator;
            var binaryOperator = instructionOperator as BinaryOperator;
            var unaryOperator = instructionOperator as UnaryOperator;

            var operationName = localOperator.Name;
            switch (operationName)
            {
                case OpcodeOperatorNames.Add:
                    HandleAdd(binaryOperator, sb, closureEntities);
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
                    throw new InvalidOperationException($"Operation '{operationName}' is not handled");
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
                    throw new InvalidOperationException($"Operation '{operationName}' is not handled");
            }
        }
        

        static void HandleClt(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} < {2})?1:0;", local, left, right);
        }

        static void HandleCgt(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} > {2})?1:0;", local, left, right);
        }

        static void HandleCeq(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} == {2})?1:0;", local, left, right);
        }

        static void HandleNeg(UnaryOperator localVar, StringBuilder sb)
        {
            var operat = localVar;
            sb.AppendFormat("{0} = -{1};", localVar.AssignedTo.Name, operat.Left.Name);
        }

        static void HandleConvR4(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (float){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        static void HandleConvR8(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (double){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        static void HandleConvI(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (void*){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        static void HandleConvU1(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (System_Byte){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        static void HandleConvI4(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (int){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        static void HandleConvI8(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (System_Int64){1};", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        static void HandleLoadLen(UnaryOperator unaryOperator, StringBuilder sb)
        {
            sb.AppendFormat("{0} = {1}->Length;", unaryOperator.AssignedTo.Name, unaryOperator.Left.Name);
        }

        static void HandleLoadArrayRef(BinaryOperator binaryOperator, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(binaryOperator, out right, out left, out local);

            sb.AppendFormat("{0}={1}[{2}];", local, right, left);
        }

        static void HandleNot(UnaryOperator localVar, StringBuilder sb)
        {
            var local = localVar.AssignedTo.Name;
            string left;
            GetUnaryOperandNames(localVar, out left);
            sb.AppendFormat("{0} = !{1};", local, left);
        }

        static void HandleXor(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}^{2};", local, left, right);
        }

        static void HandleOr(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}|{2};", local, left, right);
        }

        static void HandleAnd(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}&{2};", local, left, right);
        }

        static void HandleMul(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}*{2};", local, left, right);
        }

        static void HandleDiv(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}/{2};", local, left, right);
        }

        static void HandleRem(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}%{2};", local, left, right);
        }

        static void GetBinaryOperandNames(BinaryOperator localVar, out string right,
            out string left, out string local)
        {
            local = localVar.AssignedTo.Name;
            var leftVar = localVar.Left as LocalVariable;
            left = leftVar == null ? localVar.Left.ToString() : leftVar.Name;
            var rightVar = localVar.Right as LocalVariable;
            right = rightVar == null ? localVar.Right.ToString() : rightVar.Name;
        }

        static void GetUnaryOperandNames(UnaryOperator localVar, out string left)
        {
            left = localVar.Left.Name;
        }

        static void HandleSub(BinaryOperator localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}-{2};", local, left, right);
        }

        static void HandleAdd(BinaryOperator localVar, StringBuilder sb, ClosureEntities closureEntities)
        {
            string right, left, local;
            GetBinaryOperandNames(localVar, out right, out left, out local);
            if (localVar.Right.ComputedType().GetClrType(closureEntities) == typeof(IntPtr))
            {
                sb.AppendFormat("{0} = {1}+(size_t){2};", local, left, right);

                return;
            }

            sb.AppendFormat("{0} = {1}+{2};", local, left, right);
        }

        static void HandleSetArrayValue(LocalOperation operation, StringBuilder sb,
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

        static void HandleReadArrayItem(LocalOperation operation, StringBuilder bodySb,
            MethodInterpreter interpreter, ClosureEntities closureEntities)
        {
            var valueSrc = (GetArrayElement)operation;
            var parentType = valueSrc.Instance.ComputedType();
            var variableData = interpreter.AnalyzeProperties.GetVariableData(valueSrc.AssignedTo);
            switch (variableData)
            {
                case EscapingMode.Smart:
                    bodySb.AppendFormat(
                        (parentType.GetClrType(closureEntities).IsClass ||
                         parentType.GetClrType(closureEntities).IsInterface)
                            ? "{0} = (*{1})[{2}];"
                            : "{0} = {1}[{2}];",
                        valueSrc.AssignedTo.Name, valueSrc.Instance.Name, valueSrc.Index.Name);
                    return;
                case EscapingMode.Pointer:
                    bodySb.AppendFormat(
                        (parentType.GetClrType(closureEntities).IsClass ||
                         parentType.GetClrType(closureEntities).IsInterface)
                            ? "{0} = ((*{1})[{2}]).get();"
                            : "{0} = ({1}[{2}]).get();",
                        valueSrc.AssignedTo.Name, valueSrc.Instance.Name, valueSrc.Index.Name);

                    return;
            }
        }

        static void HandleGetField(LocalOperation operation, StringBuilder bodySb,
            MethodInterpreter interpreter)
        {
            var fieldGetterInfo = (GetField)operation;
            var assignedFrom = fieldGetterInfo.Instance;
            var assignedFromData = interpreter.AnalyzeProperties.GetVariableData(assignedFrom);
            var isOnStack = assignedFromData == EscapingMode.Stack;
            var fieldText = string.Format(isOnStack ? "{0}.{1}" : "{0}->{1}", fieldGetterInfo.Instance.Name,
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

        static void HandleSetField(LocalOperation operation, StringBuilder bodySb, ClosureEntities closureEntities)
        {
            var assign = (SetField)operation;

            if (assign.Right is ConstValue)
            {
                if (assign.FixedType.GetClrType(closureEntities) == typeof(string))
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

        static void HandleNewArray(LocalOperation operation, StringBuilder bodySb, MethodInterpreter interpreter)
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

        static void HandleNewObject(LocalOperation operation, StringBuilder bodySb, TypeDescriptionTable typeTable,
            MethodInterpreter interpreter, ClosureEntities crRuntime)
        {
            var value = (NewConstructedObject)operation;
            var rightValue = value;
            var localValue = rightValue.Info;

            var declaringType = localValue.DeclaringType;
            var targetType = declaringType.GetMappedType(crRuntime);
            var cppName = declaringType.ToDeclaredVariableType(EscapingMode.Stack);
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
            typeTable.SetIdOfInstance(bodySb, value.AssignedTo, targetType, isStack);
        }
    }
}