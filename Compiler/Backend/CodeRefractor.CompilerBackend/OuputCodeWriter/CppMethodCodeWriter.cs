#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.CompilerBackend.HandleOperations;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using Mono.Reflection;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    internal static class CppMethodCodeWriter
    {
        public static string WritePlatformInvokeMethod(MethodInterpreter platformInvoke)
        {
            var invokeRepresentation = platformInvoke.PlatformInvoke;
            var methodId = PlatformInvokeCodeWriter.Import(invokeRepresentation.LibraryName,
                                                           invokeRepresentation.MethodName);

            var sb = new StringBuilder();

            sb.AppendFormat(platformInvoke.WritePInvokeDefinition(methodId));

            sb.Append(platformInvoke.Method.WriteHeaderMethod(false));
            sb.AppendLine("{");
            var identifierValues = platformInvoke.Method.GetParameters();
            var argumentsCall = string.Join(", ", identifierValues.Select(p => p.Name));

            sb.AppendFormat("{0}({1});", methodId, argumentsCall);
            sb.AppendLine();
            sb.AppendLine("}");
            return sb.ToString();
        }

        public static string WriteCode(MetaMidRepresentation midRepresentation)
        {
            var operations = midRepresentation.LocalOperations;
            var headerSb = new StringBuilder();
            WriteSignature(midRepresentation.Method, headerSb);

            var bodySb = ComputeBodySb(operations);
            var variablesSb = ComputeVariableSb(midRepresentation);
            var finalSb = new StringBuilder();
            finalSb.AppendLine(headerSb.ToString());
            finalSb.AppendLine(variablesSb.ToString());
            finalSb.AppendLine(bodySb.ToString());
            return finalSb.ToString();
        }

        private static StringBuilder ComputeBodySb(List<LocalOperation> operations)
        {
            var bodySb = new StringBuilder();
            foreach (var operation in operations)
            {
                switch (operation.Kind)
                {
                    case LocalOperation.Kinds.Label:
                        WriteLabel(bodySb, (int) operation.Value);
                        break;
                    case LocalOperation.Kinds.Assignment:
                        StoreLocal(bodySb, operation);
                        break;

                    case LocalOperation.Kinds.BinaryOperator:
                        CppHandleOperators.HandleOperator(operation.Value, bodySb);
                        break;
                    case LocalOperation.Kinds.UnaryOperator:
                        CppHandleOperators.HandleUnaryOperator((UnaryOperator)operation.Value, bodySb);
                        break;
                    case LocalOperation.Kinds.AlwaysBranch:
                        HandleAlwaysBranchOperator(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.BranchOperator:
                        CppHandleBranches.HandleBranchOperator(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.Call:
                        CppHandleCalls.HandleCall(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.CallRuntime:
                        CppHandleCalls.HandleCallRuntime(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.Return:
                        CppHandleCalls.HandleReturn(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.NewObject:
                        HandleNewObject(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.SetField:
                        HandleSetField(operation, bodySb);
                        break;

                    case LocalOperation.Kinds.GetField:
                        HandleLoadField(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.SetStaticField:
                        HandleSetStaticField(operation, bodySb);
                        break;

                    case LocalOperation.Kinds.GetStaticField:
                        HandleLoadStaticField(operation, bodySb);
                        break;

                    case LocalOperation.Kinds.GetArrayItem:
                        HandleReadArrayItem(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.NewArray:
                        HandleNewArray(operation, bodySb);
                        break;

                    case LocalOperation.Kinds.SetArrayItem:
                        HandleSetArrayValue(operation, bodySb);
                        break;

                    case LocalOperation.Kinds.CopyArrayInitializer:
                        HandleCopyArrayInitializer(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.RefAssignment:
                        HandleRefAssignment(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.DerefAssignment:
                        HandleDerefAssignment(operation, bodySb);
                        break;

                    case LocalOperation.Kinds.Switch:
                        HandleSwitch(operation, bodySb);
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

        private static void HandleRefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (RefAssignment) operation.Value;
            var leftData = (IdentifierValue) assign.Left;
            var rightData = (IdentifierValue) assign.Right;
            bodySb.AppendFormat("{0} = &{1};", leftData.Name, rightData.Name);
        }

        private static void HandleDerefAssignment(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (DerefAssignment) operation.Value;
            var leftData = (IdentifierValue) assign.Left;
            var rightData = (IdentifierValue) assign.Right;
            bodySb.AppendFormat("{0} = *{1};", leftData.Name, rightData.Name);
        }

        private static void HandleLoadStaticField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation.Value;
            var rightData = (StaticFieldGetter) assign.Right;
            bodySb.AppendFormat("{0} = {1}::{2};", assign.Left.Name, rightData.DeclaringType.Name, rightData.FieldName);
        }

        private static void HandleSetStaticField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation.Value;
            var rightData = (StaticFieldSetter) assign.Left;
            bodySb.AppendFormat("{1}::{2} = {0};", assign.Right.Name, rightData.DeclaringType.Name, rightData.FieldName);
        }

        private static void HandleSwitch(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation.Value;
            var instructionTable = (Instruction[]) ((ConstValue) assign.Right).Value;

            var instructionLabelIds = instructionTable.Select(i => i.Offset).ToList();
            bodySb.AppendFormat("switch({0}) {{", assign.Left.Name);
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
            var assignment = (Assignment) operation.Value;
            var left = assignment.Left;
            var right = (ConstByteArrayValue) assignment.Right;
            var rightArrayData = (ConstByteArrayData) right.Value;
            var rightArray = rightArrayData.Data;
            sb.AppendFormat("{0} = std::shared_ptr< Array < System::Byte > >(new Array < System::Byte >(" +
                            "{1}, RuntimeHelpers_GetBytes({2}) ) ); ",
                            left.Name,
                            rightArray.Length,
                            right.Id);
        }

        private static void HandleSetArrayValue(LocalOperation operation, StringBuilder sb)
        {
            var assignment = (Assignment) operation.Value;
            var arrayItem = (ArrayVariable) assignment.Left;
            var right = assignment.Right;
            sb.AppendFormat("(*{0})[{1}] = {2}; ",
                            arrayItem.Parent.Name,
                            arrayItem.Index.Name,
                            right.Name);
        }

        private static void HandleNewArray(LocalOperation operation, StringBuilder bodySb)
        {
            var assignment = (Assignment) operation.Value;
            var arrayData = (NewArrayObject) assignment.Right;
            bodySb.AppendFormat("{0} = std::shared_ptr< Array < {1} > > (new Array < {1} >({2}) ); ",
                                assignment.Left.Name,
                                arrayData.TypeArray.ToCppName(),
                                arrayData.ArrayLength.Name);
        }

        private static void HandleReadArrayItem(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var valueSrc = (ArrayVariable) value.Right;
            var parentType = valueSrc.Parent.ComputedType();
            bodySb.AppendFormat(parentType.IsClass
                                    ? "{0} = (*{1})[{2}];"
                                    : "{0} = {1}[{2}];",
                                value.Left.Name, valueSrc.Parent.Name, valueSrc.Index.Name);
        }

        private static void HandleLoadArgument(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var argumentData = (ArgumentVariable) value.Right;

            bodySb.AppendFormat("{0} = {1};", value.Left.Name, argumentData.Name);
        }

        private static void HandleLoadField(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var fieldGetterInfo = (FieldGetter) value.Right;
            bodySb.AppendFormat("{0} = {1}->{2};", value.Left.Name, fieldGetterInfo.Instance.Name,
                                fieldGetterInfo.FieldName);
        }

        private static void HandleSetField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation.Value;
            var fieldSetter = (FieldSetter) assign.Left;

            bodySb.AppendFormat("{0}->{1} = {2};", fieldSetter.Instance.Name, fieldSetter.FieldName, assign.Right.Name);
        }

        private static void HandleNewObject(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var rightValue = (NewConstructedObject) value.Right;
            var localValue = rightValue.Info;
            var declaringType = localValue.DeclaringType;
            var cppNameSmart = declaringType.ToCppName();
            var cppName = declaringType.ToCppName(false);
            bodySb.AppendFormat("{1} = {0}(new {2}());", cppNameSmart, value.Left.Name, cppName).AppendLine();
            var typeData = (ClassTypeData) ProgramData.LocateType(declaringType);
            var typeNs = declaringType.Namespace;
            foreach (var methodInterpreter in typeData.Interpreters)
            {
                var constructorInfo = methodInterpreter.Method as ConstructorInfo;
                if (constructorInfo == null)
                    continue;
                if (constructorInfo.ToString() == rightValue.Info.ToString())
                {
                    bodySb.AppendFormat("{0}_{2}__{2}_ctor({1});", typeNs, value.Left.Name, declaringType.Name);
                }
            }
        }

        private static StringBuilder ComputeVariableSb(MetaMidRepresentation midRepresentation)
        {
            var variablesSb = new StringBuilder();
            foreach (var variableInfo in midRepresentation.Vars.LocalVars)
            {
                variablesSb.AppendFormat("{0} local_{1};", variableInfo.ComputedType().ToCppName(), variableInfo.Id);
                variablesSb.AppendLine();
            }
            foreach (var localVariable in midRepresentation.Vars.VirtRegs)
            {
                variablesSb.AppendFormat("{0} vreg_{1};", localVariable.ComputedType().ToCppName(), localVariable.Id);
                variablesSb.AppendLine();
            }
            return variablesSb;
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

        private static void WriteSignature(MethodBase method, StringBuilder sb)
        {
            var text = method.WriteHeaderMethod(false);
            sb.Append(text);
            sb.Append("{");
        }


        private static void StoreLocal(StringBuilder sb, LocalOperation operation)
        {
            var localVar = (Assignment) operation.Value;

            if (localVar.Right is NewConstructedObject)
            {
                HandleNewObject(operation, sb);
                return;
            }
            var left = localVar.Left.Name;
            var localVariable = localVar.Right as LocalVariable;
            if (localVariable != null)
            {
                var rightVar = localVariable;
                var right = rightVar.Name;
                sb.AppendFormat("{0} = {1};", left, right);
            }
            else
            {
                sb.AppendFormat("{0} = {1};", left, localVar.Right);
            }
        }
    }
}