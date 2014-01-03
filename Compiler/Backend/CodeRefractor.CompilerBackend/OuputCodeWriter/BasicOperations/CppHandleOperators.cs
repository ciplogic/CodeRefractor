#region Usings

using System;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter.BasicOperations
{
    internal static class CppHandleOperators
    {
        public static void HandleOperator(object operation, StringBuilder sb)
        {
            var instructionOperator = (OperatorBase)operation;
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
                    throw new InvalidOperationException(string.Format("Operation '{0}' is not handled", operationName));
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
                    throw new InvalidOperationException(string.Format("Operation '{0}' is not handled", operationName));
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
    }
}