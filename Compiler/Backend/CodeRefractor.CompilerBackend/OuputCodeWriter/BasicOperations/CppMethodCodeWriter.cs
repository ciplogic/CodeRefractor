#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.CompilerBackend.HandleOperations;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter.BasicOperations
{
    internal static class CppMethodCodeWriter
    {
        public static string WriteCode(MethodInterpreter midRepresentation)
        {
            var operations = midRepresentation.MidRepresentation.LocalOperations;
            var headerSb = new StringBuilder();
            WriteSignature(midRepresentation.Method, headerSb);

            headerSb.Append("{");
            var bodySb = ComputeBodySb(operations, midRepresentation.MidRepresentation.Vars);
            var variablesSb = ComputeVariableSb(midRepresentation.MidRepresentation);
            var finalSb = new StringBuilder();
            finalSb.AppendLine(headerSb.ToString());
            finalSb.AppendLine(variablesSb.ToString());
            finalSb.AppendLine(bodySb.ToString());
            return finalSb.ToString();
        }

        private static StringBuilder ComputeBodySb(List<LocalOperation> operations, MidRepresentationVariables vars)
        {
            var bodySb = new StringBuilder();
            foreach (var operation in operations)
            {
                switch (operation.Kind)
                {
                    case OperationKind.Label:
                        WriteLabel(bodySb, (int)operation.Value);
                        break;
                    case OperationKind.Assignment:
                        HandleAssign(bodySb, operation, vars);
                        break;

                    case OperationKind.BinaryOperator:
                        CppHandleOperators.HandleOperator(operation.Value, bodySb);
                        break;
                    case OperationKind.UnaryOperator:
                        CppHandleOperators.HandleUnaryOperator((UnaryOperator)operation.Value, bodySb);
                        break;
                    case OperationKind.AlwaysBranch:
                        HandleAlwaysBranchOperator(operation, bodySb);
                        break;
                    case OperationKind.BranchOperator:
                        CppHandleBranches.HandleBranchOperator(operation, bodySb);
                        break;
                    case OperationKind.Call:
                        CppHandleCalls.HandleCall(operation, bodySb, vars);
                        break;
                    case OperationKind.CallRuntime:
                        CppHandleCalls.HandleCallRuntime(operation, bodySb);
                        break;
                    case OperationKind.Return:
                        CppHandleCalls.HandleReturn(operation, bodySb);
                        break;
                    case OperationKind.NewObject:
                        HandleNewObject(operation, bodySb,vars);
                        break;
                    case OperationKind.SetField:
                        HandleSetField(operation, bodySb);
                        break;

                    case OperationKind.GetField:
                        HandleLoadField(operation, bodySb, vars);
                        break;
                    case OperationKind.SetStaticField:
                        HandleSetStaticField(operation, bodySb);
                        break;

                    case OperationKind.GetStaticField:
                        HandleLoadStaticField(operation, bodySb);
                        break;

                    case OperationKind.GetArrayItem:
                        HandleReadArrayItem(operation, bodySb, vars);
                        break;
                    case OperationKind.NewArray:
                        HandleNewArray(operation, bodySb, vars);
                        break;

                    case OperationKind.SetArrayItem:
                        HandleSetArrayValue(operation, bodySb);
                        break;

                    case OperationKind.CopyArrayInitializer:
                        HandleCopyArrayInitializer(operation, bodySb);
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
                    case OperationKind.Switch:
                        HandleSwitch(operation, bodySb);
                        break;

                    case OperationKind.LoadFunction:
                        HandleLoadFunction(operation, bodySb);
                        break;
                    case OperationKind.SizeOf:
                        HandleSizeOf(operation, bodySb);
                        break;
                    case OperationKind.Comment:
                        HandleComment(operation.Value.ToString(), bodySb);
                        break;

                    default:
                        throw new InvalidOperationException(
                            string.Format(
                                "Invalid operation '{0}' is introduced in intermediary representation\nValue: {1}",
                                operation.Kind,
                                operation.Value));
                }
                bodySb.AppendLine();
            }
            bodySb.AppendLine("}");
            return bodySb;
        }

        private static void HandleComment(string toString, StringBuilder bodySb)
        {
            bodySb
                .AppendFormat("// {0}", toString);
        }

        private static void HandleGetAddressOfArrayItem(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (RefArrayItemAssignment)operation.Value;
            bodySb.AppendFormat("{0} = & ({1}->Items[{2}]);", value.Left.Name, value.ArrayVar.Name, value.Index.Name);
        }

        private static void HandleLoadFunction(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (FunctionPointerStore)operation.Value;
            var leftData = assign.AssignedTo;
            var info = assign.FunctionPointer;
            var methodName = info.ClangMethodSignature();
            bodySb.AppendFormat("{0}=&({1});", leftData.Name, methodName);

        }

        private static void HandleSizeOf(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (SizeOfAssignment)operation.Value;
            var leftData = (IdentifierValue)assign.AssignedTo;
            var rightData = assign.Right.ToCppName();
            bodySb.AppendFormat("{0} = sizeof({1});", leftData.Name, rightData);
        }

        private static void HandleRefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (RefAssignment)operation.Value;
            var leftData = (IdentifierValue)assign.Left;
            var rightData = (IdentifierValue)assign.Right;
            bodySb.AppendFormat("{0} = &{1};", leftData.Name, rightData.Name);
        }

        private static void HandleFieldRefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (FieldRefAssignment)operation.Value;
            var leftData = assign.Left;
            var rightData = assign.Right;
            var fieldName = assign.Field.Name;
            bodySb.AppendFormat("{0} = &{1}->{2};", leftData.Name, rightData.Name, fieldName);
        }

        private static void HandleDerefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (DerefAssignment)operation.Value;
            var leftData = (IdentifierValue)assign.Left;
            var rightData = (IdentifierValue)assign.Right;
            bodySb.AppendFormat("{0} = *{1};", leftData.Name, rightData.Name);
        }

        private static void HandleLoadStaticField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment)operation.Value;
            var rightData = (StaticFieldGetter)assign.Right;
            bodySb.AppendFormat("{0} = {1}::{2};", assign.AssignedTo.Name,
                rightData.DeclaringType.ClrType.ToCppMangling(),
                rightData.FieldName.ValidName());
        }

        private static void HandleSetStaticField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment)operation.Value;
            var rightData = (StaticFieldSetter)assign.AssignedTo;
            bodySb.AppendFormat("{1}::{2} = {0};", assign.Right.Name,
                rightData.DeclaringType.ToCppMangling(),
                rightData.FieldName.ValidName());
        }

        private static void HandleSwitch(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment)operation.Value;
            var instructionTable = (int[])((ConstValue)assign.Right).Value;

            var instructionLabelIds = instructionTable;
            bodySb.AppendFormat("switch({0}) {{", assign.AssignedTo.Name);
            bodySb.AppendLine();
            var pos = 0;
            foreach (var instructionLabelId in instructionLabelIds)
            {
                bodySb.AppendFormat("case {0}:", pos++);
                bodySb.AppendFormat("\tgoto label_{0};", instructionLabelId);
                bodySb.AppendLine();
            }
            bodySb.AppendLine("}");
        }

        private static void HandleCopyArrayInitializer(LocalOperation operation, StringBuilder sb)
        {
            var assignment = (Assignment)operation.Value;
            var left = assignment.AssignedTo;
            var right = (ConstByteArrayValue)assignment.Right;
            var rightArrayData = (ConstByteArrayData)right.Value;
            var rightArray = rightArrayData.Data;
            sb.AppendFormat("{0} = std::make_shared<Array<System::Byte> >(" +
                            "{1}, RuntimeHelpers_GetBytes({2}) ); ",
                            left.Name,
                            rightArray.Length,
                            right.Id);
        }

        private static void HandleSetArrayValue(LocalOperation operation, StringBuilder sb)
        {
            var assignment = (Assignment)operation.Value;
            var arrayItem = (ArrayVariable)assignment.AssignedTo;
            var right = assignment.Right;
            sb.AppendFormat("(*{0})[{1}] = {2}; ",
                            arrayItem.Parent.Name,
                            arrayItem.Index.Name,
                            right.Name);
        }

        private static void HandleReadArrayItem(LocalOperation operation, StringBuilder bodySb, MidRepresentationVariables vars)
        {
            var value = (Assignment)operation.Value;
            var valueSrc = (ArrayVariable)value.Right;
            var parentType = valueSrc.Parent.ComputedType();
            var variableData = vars.GetVariableData(value.AssignedTo);
            switch (variableData.Escaping)
            {
                case EscapingMode.Smart:
                    bodySb.AppendFormat(parentType.ClrType.IsClass
                                            ? "{0} = (*{1})[{2}];"
                                            : "{0} = {1}[{2}];",
                                        value.AssignedTo.Name, valueSrc.Parent.Name, valueSrc.Index.Name);
                    return;
                case EscapingMode.Pointer:
                    bodySb.AppendFormat(parentType.ClrType.IsClass
                                            ? "{0} = ((*{1})[{2}]).get();"
                                            : "{0} = ({1}[{2}]).get();",
                                        value.AssignedTo.Name, valueSrc.Parent.Name, valueSrc.Index.Name);

                    return;

            }
        }

        private static void HandleLoadField(LocalOperation operation, StringBuilder bodySb, MidRepresentationVariables vars)
        {
            var fieldGetterInfo = (FieldGetter)operation.Value;
            var assignedFrom = fieldGetterInfo.Instance;
            var assignedFromData = vars.GetVariableData(assignedFrom);
            var getStackField = assignedFromData.Escaping == EscapingMode.Stack;
            var fieldText = string.Format(getStackField ? "{0}.{1}" : "{0}->{1}", fieldGetterInfo.Instance.Name,
                fieldGetterInfo.FieldName);

            var assignedTo = fieldGetterInfo.AssignedTo;
            var assignedToData = vars.GetVariableData(assignedTo);
            switch (assignedToData.Escaping)
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
            var assign = (Assignment)operation.Value;
            var fieldSetter = (FieldSetter)assign.AssignedTo;

            bodySb.AppendFormat("{0}->{1} = {2};", fieldSetter.Instance.Name,
                fieldSetter.FieldName.ValidName(), assign.Right.Name);
        }

        private static void HandleNewArray(LocalOperation operation, StringBuilder bodySb,
            MidRepresentationVariables vars)
        {
            var assignment = (Assignment) operation.Value;
            var arrayData = (NewArrayObject) assignment.Right;

            var assignedData = vars.GetVariableData(assignment.AssignedTo);
            switch (assignedData.Escaping)
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


        private static void HandleNewObject(LocalOperation operation, StringBuilder bodySb, MidRepresentationVariables vars)
        {
            var value = (Assignment)operation.Value;
            var rightValue = (NewConstructedObject)value.Right;
            var localValue = rightValue.Info;

            var declaringType = localValue.DeclaringType;
            var cppName = declaringType.ToCppName(EscapingMode.Stack);
            var assignedData = vars.GetVariableData(value.AssignedTo);
            switch (assignedData.Escaping)
            {
                case EscapingMode.Stack:
                    bodySb.AppendFormat("{1} {0};", value.AssignedTo.Name, cppName);

                    break;
                default:
                    bodySb.AppendFormat("{0} = std::make_shared<{1}>();", value.AssignedTo.Name, cppName);
                    break;
            }
        }

        private static StringBuilder ComputeVariableSb(MetaMidRepresentation midRepresentation)
        {
            var variablesSb = new StringBuilder();
            var vars = midRepresentation.Vars;
            foreach (var variableInfo in vars.LocalVars)
            {
                AddVariableContent(variablesSb, "{0} local_{1};", variableInfo, vars);
            }
            foreach (var localVariable in vars.VirtRegs)
            {
                AddVariableContent(variablesSb, "{0} vreg_{1};", localVariable, vars);
            }
            return variablesSb;
        }

        static string ComputeCommaSeparatedParameterTypes(LocalVariable localVariable)
        {
            var methodInfo = (MethodInfo)localVariable.CustomData;

            var parameters = methodInfo.GetMethodArgumentTypes().ToArray();

            var parametersFormat = TypeNamerUtils.GetCommaSeparatedParameters(parameters);
            return parametersFormat;
        }

        private static void AddVariableContent(StringBuilder variablesSb, string format, LocalVariable localVariable, MidRepresentationVariables vars)
        {
            var localVariableData = vars.GetVariableData(localVariable);
            if (localVariableData.Escaping == EscapingMode.Stack)
                return;
            if (localVariable.ComputedType().ClrType.IsSubclassOf(typeof(MethodInfo)))
            {
                variablesSb
                    .AppendFormat("void (*{0})({1});",
                        localVariable.Name,
                        ComputeCommaSeparatedParameterTypes(localVariable))
                    .AppendLine();
                return;
            }
            if (localVariableData.Escaping == EscapingMode.Pointer)
            {
                var cppName = localVariable.ComputedType()
                    .ClrType.ToCppName(localVariableData.Escaping);
                variablesSb
                    .AppendFormat(format, cppName, localVariable.Id)
                    .AppendLine();
                return;
            }
            variablesSb
                .AppendFormat(format, localVariable.ComputedType()
                .ClrType.ToCppName(localVariableData.Escaping), localVariable.Id)
                .AppendLine();
        }

        private static void HandleAlwaysBranchOperator(LocalOperation operation, StringBuilder sb)
        {
            sb.AppendFormat("goto label_{0};", operation.Value);
        }


        private static void WriteLabel(StringBuilder sb, int value)
        {
            sb.AppendFormat("label_{0}:", value);
        }

        #region Call

        #endregion

        internal static void WriteSignature(MethodBase method, StringBuilder sb, bool writeEndColon = false)
        {
            if (method == null)
                return;
            var text = method.WriteHeaderMethodWithEscaping(writeEndColon);
            sb.Append(text);
        }


        private static void HandleAssign(StringBuilder sb, LocalOperation operation, MidRepresentationVariables vars)
        {
            var assignment = (Assignment)operation.Value;

            if (assignment.Right is NewConstructedObject)
            {
                HandleNewObject(operation, sb, vars);
                return;
            }
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
                var assignedToData = vars.GetVariableData(assignedTo);
                var localVariableData = vars.GetVariableData(localVariable);
                var rightVar = localVariable;
                if (assignedToData.Escaping == localVariableData.Escaping 
                    || assignedTo.ComputedType().ClrTypeCode!=TypeCode.Object)
                {
                    sb.AppendFormat("{0} = {1};", assignedTo.Name, rightVar.Name);
                    return;
                }
                switch (assignedToData.Escaping)
                {
                    case EscapingMode.Pointer:
                        switch (localVariableData.Escaping)
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
                        break;

                }
                throw new InvalidDataException("Case not handled");
            }
            else
            {
                sb.AppendFormat("{0} = {1};", assignedTo.Name, assignment.Right.ComputedValue());
            }
        }
    }
}