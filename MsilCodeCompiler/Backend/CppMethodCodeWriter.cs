#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.Compiler.Shared;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable;
using CodeRefractor.RuntimeBase.Shared;
using Mono.Reflection;

#endregion

namespace CodeRefractor.Compiler.Backend
{
    internal class CppMethodCodeWriter
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

        public string WriteCode(MetaMidRepresentation midRepresentation)
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

        private StringBuilder ComputeBodySb(List<LocalOperation> operations)
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

                    case LocalOperation.Kinds.Operator:
                        HandleOperator(operation.Value, bodySb);
                        break;
                    case LocalOperation.Kinds.AlwaysBranch:
                        HandleAlwaysBranchOperator(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.BranchOperator:
                        HandleBranchOperator(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.Call:
                        HandleCall(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.CallRuntime:
                        HandleCallRuntime(operation, bodySb);
                        break;
                    case LocalOperation.Kinds.Return:
                        HandleReturn(operation, bodySb);
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

                    case LocalOperation.Kinds.LoadArgument:
                        HandleLoadArgument(operation, bodySb);
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

        private void HandleSwitch(LocalOperation operation, StringBuilder bodySb)
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

        private void HandleCopyArrayInitializer(LocalOperation operation, StringBuilder sb)
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

        private void HandleSetArrayValue(LocalOperation operation, StringBuilder sb)
        {
            var assignment = (Assignment) operation.Value;
            var arrayItem = (ArrayVariable) assignment.Left;
            var right = assignment.Right;
            sb.AppendFormat("(*{0})[{1}] = {2}; ",
                            arrayItem.Parent.Name,
                            arrayItem.Index.Name,
                            right.Name);
        }

        private void HandleNewArray(LocalOperation operation, StringBuilder bodySb)
        {
            var assignment = (Assignment) operation.Value;
            var arrayData = (NewArrayObject) assignment.Right;
            bodySb.AppendFormat("{0} = std::shared_ptr< Array < {1} > > (new Array < {1} >({2}) ); ",
                                assignment.Left.Name,
                                arrayData.TypeArray.ToCppName(),
                                arrayData.ArrayLength.Name);
        }

        private static void HandleReturn(LocalOperation operation, StringBuilder bodySb)
        {
            var returnValue = operation.Value as IdentifierValue;

            if (returnValue == null)
                bodySb.Append("return;");
            else
                bodySb.AppendFormat("return {0};", returnValue.Name);
        }

        private void HandleReadArrayItem(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var valueSrc = (ArrayVariable) value.Right;
            var parentType = valueSrc.Parent.ComputedType();
            bodySb.AppendFormat(parentType.IsClass
                                    ? "{0} = (*{1})[{2}];"
                                    : "{0} = {1}[{2}];",
                                value.Left.Name, valueSrc.Parent.Name, valueSrc.Index.Name);
        }

        private void HandleLoadArgument(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var argumentData = (ArgumentVariable) value.Right;

            bodySb.AppendFormat("{0} = {1};", value.Left.Name, argumentData.Name);
        }

        private void HandleLoadField(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var fieldGetterInfo = (FieldGetter) value.Right;
            bodySb.AppendFormat("{0} = {1}->{2};", value.Left.Name, fieldGetterInfo.Instance.Name,
                                fieldGetterInfo.FieldName);
        }

        private void HandleSetField(LocalOperation operation, StringBuilder bodySb)
        {
            var assign = (Assignment) operation.Value;
            var fieldSetter = (FieldSetter) assign.Left;

            bodySb.AppendFormat("{0}->{1} = {2};", fieldSetter.Instance.Name, fieldSetter.FieldName, assign.Right.Name);
        }

        private void HandleNewObject(LocalOperation operation, StringBuilder bodySb)
        {
            var value = (Assignment) operation.Value;
            var rightValue = (NewConstructedObject) value.Right;
            var localValue = rightValue.Info;
            var declaringType = localValue.DeclaringType;
            var cppNameSmart = declaringType.ToCppName();
            var cppName = declaringType.ToCppName(false);
            bodySb.AppendFormat("{1} = {0}(new {2}());", cppNameSmart, value.Left.Name, cppName).AppendLine();
            var typeData = ProgramData.Instance.LocateType(declaringType);
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

        private void HandleAlwaysBranchOperator(LocalOperation operation, StringBuilder sb)
        {
            sb.AppendFormat("goto label_{0};", operation.Value);
        }


        private void HandleClt(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} < {2})?1:0;", local, left, right);
        }

        private void HandleCgt(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} > {2})?1:0;", local, left, right);
        }

        private void HandleCeq(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} == {2})?1:0;", local, left, right);
        }

        private void WriteLabel(StringBuilder sb, int value)
        {
            sb.AppendFormat("label_{0}:", value);
        }

        #region Call

        private static void HandleCall(LocalOperation operation, StringBuilder sb)
        {
            var operationData = (MethodData) operation.Value;

            var methodInfo = operationData.Info;
            if (methodInfo.IsConstructor)
                return; //don't call constructor for now
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (isVoidMethod)
            {
                sb.AppendFormat("{0}", methodInfo.ClangMethodSignature());
            }
            else
            {
                sb.AppendFormat("{1} = {0}", methodInfo.ClangMethodSignature(),
                                operationData.Result.Name);
            }
            var identifierValues = operationData.Parameters;
            var argumentsCall = string.Join(", ", identifierValues.Select(p => p.Name));

            sb.AppendFormat("({0});", argumentsCall);
        }


        private void HandleCallRuntime(LocalOperation operation, StringBuilder sb)
        {
            var operationData = (MethodData) operation.Value;

            var methodInfo = operationData.Info;
            if (methodInfo.IsConstructor)
                return; //don't call constructor for now
            var isVoidMethod = methodInfo.GetReturnType().IsVoid();
            if (isVoidMethod)
            {
                sb.AppendFormat("{0}", methodInfo.ClangMethodSignature());
            }
            else
            {
                sb.AppendFormat("{1} = {0}", methodInfo.ClangMethodSignature(),
                                operationData.Result.Name);
            }
            var identifierValues = operationData.Parameters;
            var argumentsCall = string.Join(", ", identifierValues.Select(p => p.Name));

            sb.AppendFormat("({0});", argumentsCall);
        }

        #endregion

        private void WriteSignature(MethodBase method, StringBuilder sb)
        {
            var text = method.WriteHeaderMethod(false);
            sb.Append(text);
            sb.Append("{");
        }


        private void HandleBranchOperator(object operation, StringBuilder sb)
        {
            var localBranch = (LocalOperation) operation;
            var objList = (BranchOperator) localBranch.Value;
            var operationName = objList.Name;
            var jumpAddress = objList.JumpTo;
            var localVar = objList.CompareValue;
            var secondVar = objList.SecondValue;
            switch (operationName)
            {
                case OpcodeBranchNames.BrTrue:
                    HandleBrTrue(localVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.BrFalse:
                    HandleBrFalse(localVar, sb, jumpAddress);
                    break;

                case OpcodeBranchNames.Beq:
                    HandleBeq(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Bge:
                    HandleBge(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Bgt:
                    HandleBgt(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Ble:
                    HandleBle(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Blt:
                    HandleBlt(localVar, secondVar, sb, jumpAddress);
                    break;
                case OpcodeBranchNames.Bne:
                    HandleBne(localVar, secondVar, sb, jumpAddress);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Operation '{0}' is not handled", operationName));
            }
        }

        private void HandleBne(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb, int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "!=");
        }

        private void HandleBlt(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb, int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "<");
        }

        private void HandleBle(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb, int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "<=");
        }

        private void HandleBgt(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb, int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, ">");
        }

        private void HandleBge(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb, int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, ">=");
        }

        private void HandleBeq(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb, int jumpAddress)
        {
            WriteCompareBranch(localVar, secondVar, sb, jumpAddress, "==");
        }

        private void WriteCompareBranch(IdentifierValue localVar, IdentifierValue secondVar, StringBuilder sb,
                                        int jumpAddress,
                                        string comparisonOperator)
        {
            var local = localVar.Name;
            var second = secondVar.Name;
            sb.AppendFormat("if({0}{3}{1}) goto label_{2};", local, second, jumpAddress, comparisonOperator);
        }

        private void HandleBrFalse(IdentifierValue localVar, StringBuilder sb, int jumpAddress)
        {
            var local = localVar.Name;
            sb.AppendFormat("if(!({0})) goto label_{1};", local, jumpAddress);
        }

        private void HandleBrTrue(IdentifierValue localVar, StringBuilder sb, int jumpAddress)
        {
            var local = localVar.Name;
            sb.AppendFormat("if({0}) goto label_{1};", local, jumpAddress);
        }

        private void HandleOperator(object operation, StringBuilder sb)
        {
            var localVar = (Assignment) operation;
            var localOperator = (Operator) localVar.Right;
            var binaryOperator = localVar.Right as BinaryOperator;
            var unaryOperator = localVar.Right as UnaryOperator;

            var operationName = localOperator.Name;
            switch (operationName)
            {
                case OpcodeOperatorNames.Add:
                    HandleAdd(binaryOperator, localVar, sb);
                    break;
                case OpcodeOperatorNames.Sub:
                    HandleSub(binaryOperator, localVar, sb);
                    break;
                case OpcodeOperatorNames.Mul:
                    HandleMul(binaryOperator, localVar, sb);
                    break;
                case OpcodeOperatorNames.Div:
                    HandleDiv(binaryOperator, localVar, sb);
                    break;
                case OpcodeOperatorNames.Rem:
                    HandleRem(binaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.Ceq:
                    HandleCeq(binaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.Cgt:
                    HandleCgt(binaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.Clt:
                    HandleClt(binaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.And:
                    HandleAnd(binaryOperator, localVar, sb);
                    break;
                case OpcodeOperatorNames.Or:
                    HandleOr(binaryOperator, localVar, sb);
                    break;
                case OpcodeOperatorNames.Xor:
                    HandleXor(binaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.Not:
                    HandleNot(localVar, sb);
                    break;
                case OpcodeOperatorNames.Neg:
                    HandleNeg(localVar, sb);
                    break;

                case OpcodeOperatorNames.LoadArrayRef:
                    HandleLoadArrayRef(binaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.LoadLen:
                    HandleLoadLen(unaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.ConvI4:
                    HandleConvI4(unaryOperator, localVar, sb);
                    break;

                case OpcodeOperatorNames.ConvR8:
                    HandleConvR8(unaryOperator, localVar, sb);
                    break;

                default:
                    throw new InvalidOperationException(string.Format("Operation '{0}' is not handled", operationName));
            }
        }

        private void HandleNeg(Assignment localVar, StringBuilder sb)
        {
            var operat = (UnaryOperator) localVar.Right;
            sb.AppendFormat("{0} = -{1};", localVar.Left.Name, operat.Left.Name);
        }

        private void HandleConvR8(UnaryOperator unaryOperator, Assignment localVar, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (double){1};", localVar.Left.Name, unaryOperator.Left.Name);
        }

        private void HandleConvI4(UnaryOperator unaryOperator, Assignment localVar, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (int){1};", localVar.Left.Name, unaryOperator.Left.Name);
        }

        private void HandleLoadLen(UnaryOperator unaryOperator, Assignment assignment, StringBuilder sb)
        {
            sb.AppendFormat("{0} = {1}->Length;", assignment.Left.Name, unaryOperator.Left.Name);
        }

        private void HandleLoadArrayRef(BinaryOperator binaryOperator, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(binaryOperator, localVar, out right, out left, out local);

            sb.AppendFormat("{0}={1}[{2}];", local, right, left);
        }

        private void HandleNot(Assignment localVar, StringBuilder sb)
        {
            string left, local;
            GetUnaryOperandNames(localVar, out left, out local);
            sb.AppendFormat("{0} = !{1};", local, left);
        }

        private void HandleXor(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}^{2};", local, left, right);
        }

        private void HandleOr(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}|{2};", local, left, right);
        }

        private void HandleAnd(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}&{2};", local, left, right);
        }

        private static void HandleMul(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}*{2};", local, left, right);
        }

        private static void HandleDiv(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}/{2};", local, left, right);
        }

        private static void HandleRem(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}%{2};", local, left, right);
        }

        private static void GetBinaryOperandNames(BinaryOperator objList, Assignment localVar, out string right,
                                                  out string left, out string local)
        {
            local = localVar.Left.Name;
            var leftVar = objList.Left as LocalVariable;
            left = leftVar == null ? objList.Left.ToString() : leftVar.Name;
            var rightVar = objList.Right as LocalVariable;
            right = rightVar == null ? objList.Right.ToString() : rightVar.Name;
        }

        private static void GetUnaryOperandNames(Assignment localVar, out string left, out string local)
        {
            left = localVar.Left.Name;
            local = localVar.Right.Name;
        }

        private static void HandleSub(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}-{2};", local, left, right);
        }

        private static void HandleAdd(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}+{2};", local, left, right);
        }

        private void StoreLocal(StringBuilder sb, LocalOperation operation)
        {
            var localVar = (Assignment) operation.Value;

            if (localVar.Right is NewConstructedObject)
            {
                HandleNewObject(operation, sb);
                return;
            }
            var left = localVar.Left.Name;
            if (localVar.Right is LocalVariable)
            {
                var rightVar = (LocalVariable) localVar.Right;
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