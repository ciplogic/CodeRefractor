#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.Runtime;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.TypeInfoWriter;

#endregion

namespace CodeRefractor.RuntimeBase.CodeWriter.BasicOperations
{
    public static class CppMethodCodeWriter
    {
        public static string WriteCode(MethodInterpreter interpreter, TypeDescriptionTable typeTable,
            CrRuntimeLibrary crRuntime)
        {
            var operations = interpreter.MidRepresentation.LocalOperations;
            var headerSb = new StringBuilder();
            var sb = CppWriteSignature.WriteSignature(interpreter, crRuntime);
            headerSb.AppendLine(sb.ToString());
            headerSb.Append("{");
            var bodySb = ComputeBodySb(operations, interpreter.MidRepresentation.Vars, typeTable, interpreter, crRuntime);
            var variablesSb = ComputeVariableSb(interpreter.MidRepresentation, interpreter);
            var finalSb = new StringBuilder();
            finalSb.AppendLine(headerSb.ToString());
            finalSb.AppendLine(variablesSb.ToString());
            finalSb.AppendLine(bodySb.ToString());
            return finalSb.ToString();
        }

        private static StringBuilder ComputeBodySb(List<LocalOperation> operations, MidRepresentationVariables vars,
            TypeDescriptionTable typeTable, MethodInterpreter interpreter, CrRuntimeLibrary crRuntime)
        {
            var bodySb = new StringBuilder();
            foreach (var operation in operations)
            {
                if (CppHandleOperators.HandleAssignmentOperations(vars, bodySb, operation, operation.Kind, typeTable,
                    interpreter))
                {
                    bodySb.AppendLine();
                    continue;
                }
                switch (operation.Kind)
                {
                    case OperationKind.Label:
                        WriteLabel(bodySb, ((Label) operation).JumpTo);
                        break;
                    case OperationKind.AlwaysBranch:
                        HandleAlwaysBranchOperator(operation, bodySb);
                        break;
                    case OperationKind.BranchOperator:
                        CppHandleBranches.HandleBranchOperator(operation, bodySb);
                        break;
                    case OperationKind.Call:
                        CppHandleCalls.HandleCall(operation, bodySb, vars, interpreter, crRuntime);
                        break;
                    case OperationKind.CallInterface:
                        CppHandleCalls.HandleCallInterface(operation, bodySb, vars, interpreter, crRuntime);
                        break;
                    case OperationKind.CallVirtual:
                        CppHandleCalls.HandleCallVirtual(operation, bodySb, vars, interpreter, crRuntime);
                        break;
                    case OperationKind.CallRuntime:
                        CppHandleCalls.HandleCallRuntime(operation, bodySb);
                        break;
                    case OperationKind.Return:
                        CppHandleCalls.HandleReturn(operation, bodySb);
                        break;

                    case OperationKind.CopyArrayInitializer:
                        HandleCopyArrayInitializer(operation, bodySb);
                        break;

                    case OperationKind.Switch:
                        HandleSwitch(operation, bodySb);
                        break;

                    case OperationKind.Comment:
                        HandleComment(operation.ToString(), bodySb);
                        break;
                    case OperationKind.Box:
                        HandleBox((Boxing)operation, bodySb, typeTable);
                        break;

                    case OperationKind.CastClass:
                        HandleCastClass((ClassCasting)operation, bodySb, typeTable);
                        break;

                    case OperationKind.Unbox:
                        HandleUnbox((Unboxing)operation, bodySb, typeTable);
                        break;

                    default:
                        throw new InvalidOperationException(
                            string.Format(
                                "Invalid operation '{0}' is introduced in intermediary representation\nValue: {1}",
                                operation.Kind,
                                operation));
                }
                bodySb.AppendLine();
            }
            bodySb.AppendLine("}");
            return bodySb;
        }

        private static void HandleUnbox(Unboxing unboxing, StringBuilder bodySb, TypeDescriptionTable typeTable)
        {
            var typeDescription = unboxing.AssignedTo.ComputedType();
            bodySb
                .AppendFormat("{0} = unbox_value<{2}>({1});",
                    unboxing.AssignedTo.Name,
                    unboxing.Right.Name,
                    typeDescription.ClrType.ToDeclaredVariableType(true, EscapingMode.Stack));
        }
        private static void HandleBox(Boxing boxing, StringBuilder bodySb, TypeDescriptionTable typeTable)
        {
            TypeDescription typeDescription = boxing.Right.ComputedType();
            bodySb
                .AppendFormat("{0} = box_value<{2}>({1}, {3});",
                    boxing.AssignedTo.Name,
                    boxing.Right.Name,
                    typeDescription.ClrType.ToDeclaredVariableType(true, EscapingMode.Stack),
                    typeTable.GetTypeId(typeDescription.ClrType));
        }


        private static void HandleCastClass(ClassCasting casting, StringBuilder bodySb, TypeDescriptionTable typeTable)
        {
            var typeDescription = casting.AssignedTo.ComputedType();
            bodySb
                .AppendFormat("{0} = std::static_pointer_cast<{2}>({1});",
                    casting.AssignedTo.Name,
                    casting.Value.Name,
                    typeDescription.ClrType.ToDeclaredVariableType(true, EscapingMode.Stack));
        }
        
        private static void HandleComment(string toString, StringBuilder bodySb)
        {
            bodySb
                .AppendFormat("// {0}", toString);
        }

        private static void HandleSwitch(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation;
            var instructionTable = (int[]) ((ConstValue) assign.Right).Value;

            var instructionLabelIds = instructionTable;
            bodySb.AppendFormat("switch({0}) {{", assign.AssignedTo.Name);
            bodySb.AppendLine();
            var pos = 0;
            foreach (var instructionLabelId in instructionLabelIds)
            {
                bodySb.AppendFormat("case {0}:", pos++);
                bodySb.AppendFormat("\tgoto label_{0};", instructionLabelId.ToHex());
                bodySb.AppendLine();
            }
            bodySb.AppendLine("}");
        }

        private static void HandleCopyArrayInitializer(LocalOperation operation, StringBuilder sb)
        {
            var assignment = (Assignment) operation;
            var left = assignment.AssignedTo;
            var right = (ConstByteArrayValue) assignment.Right;
            var rightArrayData = (ConstByteArrayData) right.Value;
            var rightArray = rightArrayData.Data;
            sb.AppendFormat("{0} = std::make_shared<Array<System::Byte> >(" +
                            "{1}, RuntimeHelpers_GetBytes({2}) ); ",
                left.Name,
                rightArray.Length,
                right.Id);
        }

        private static StringBuilder ComputeVariableSb(MetaMidRepresentation midRepresentation,
            MethodInterpreter interpreter)
        {
            var variablesSb = new StringBuilder();
            var vars = midRepresentation.Vars;
            foreach (var variableInfo in vars.LocalVars)
            {
                AddVariableContent(variablesSb, "{0} local_{1};", variableInfo, vars, interpreter);
            }
            foreach (var localVariable in vars.VirtRegs)
            {
                AddVariableContent(variablesSb, "{0} vreg_{1};", localVariable, vars, interpreter);
            }
            return variablesSb;
        }

        private static string ComputeCommaSeparatedParameterTypes(LocalVariable localVariable)
        {
            var methodInfo = (MethodInfo) localVariable.CustomData;

            var parameters = methodInfo.GetMethodArgumentTypes().ToArray();

            var parametersFormat = TypeNamerUtils.GetCommaSeparatedParameters(parameters);
            return parametersFormat;
        }

        private static void AddVariableContent(StringBuilder variablesSb, string format, LocalVariable localVariable,
            MidRepresentationVariables vars, MethodInterpreter interpreter)
        {
            var localVariableData = interpreter.AnalyzeProperties.GetVariableData(localVariable);
            if (localVariableData == EscapingMode.Stack)
                return;
            if (localVariable.ComputedType().ClrType.IsSubclassOf(typeof (MethodInfo)))
            {
                variablesSb
                    .AppendFormat("System_Void (*{0})({1}*);", //TODO: Added * to deal with pointers, is this the right approach ?
                        localVariable.Name,
                        ComputeCommaSeparatedParameterTypes(localVariable))
                    .AppendLine();
                return;
            }
            if (localVariableData == EscapingMode.Pointer)
            {
                var cppName = localVariable.ComputedType()
                    .ClrType.ToDeclaredVariableType(true, localVariableData);
                variablesSb
                    .AppendFormat(format, cppName, localVariable.Id)
                    .AppendLine();
                return;
            }
            variablesSb
                .AppendFormat(format, localVariable.ComputedType()
                    .ClrType.ToDeclaredVariableType(true, localVariableData), localVariable.Id)
                .AppendLine();
        }

        private static void HandleAlwaysBranchOperator(LocalOperation operation, StringBuilder sb)
        {
            sb.AppendFormat("goto label_{0};", ((AlwaysBranch)operation).JumpTo.ToHex());
        }


        private static void WriteLabel(StringBuilder sb, int value)
        {
            sb.AppendFormat("label_{0}:", value.ToHex());
        }

        #region Call

        #endregion
    }
}