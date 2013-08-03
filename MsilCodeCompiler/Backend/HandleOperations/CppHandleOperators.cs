using System;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.Compiler.Backend.HandleOperations
{
    internal static class CppHandleOperators
    {
        public static void HandleOperator(object operation, StringBuilder sb)
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

        public static void HandleUnaryOperator(object operation, StringBuilder sb)
        {
            var localVar = (Assignment)operation;
            var localOperator = (Operator)localVar.Right;
            var unaryOperator = localVar.Right as UnaryOperator;

            var operationName = localOperator.Name;
            switch (operationName)
            {
                case OpcodeOperatorNames.Not:
                    HandleNot(localVar, sb);
                    break;
                case OpcodeOperatorNames.Neg:
                    HandleNeg(localVar, sb);
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

        private static void HandleClt(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} < {2})?1:0;", local, left, right);
        }

        private static void HandleCgt(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} > {2})?1:0;", local, left, right);
        }

        private static void HandleCeq(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = ({1} == {2})?1:0;", local, left, right);
        }

        private static void HandleNeg(Assignment localVar, StringBuilder sb)
        {
            var operat = (UnaryOperator) localVar.Right;
            sb.AppendFormat("{0} = -{1};", localVar.Left.Name, operat.Left.Name);
        }

        private static void HandleConvR8(UnaryOperator unaryOperator, Assignment localVar, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (double){1};", localVar.Left.Name, unaryOperator.Left.Name);
        }

        private static void HandleConvI4(UnaryOperator unaryOperator, Assignment localVar, StringBuilder sb)
        {
            sb.AppendFormat("{0} = (int){1};", localVar.Left.Name, unaryOperator.Left.Name);
        }

        private static void HandleLoadLen(UnaryOperator unaryOperator, Assignment assignment, StringBuilder sb)
        {
            sb.AppendFormat("{0} = {1}->Length;", assignment.Left.Name, unaryOperator.Left.Name);
        }

        private static void HandleLoadArrayRef(BinaryOperator binaryOperator, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(binaryOperator, localVar, out right, out left, out local);

            sb.AppendFormat("{0}={1}[{2}];", local, right, left);
        }

        private static void HandleNot(Assignment localVar, StringBuilder sb)
        {
            string left, local;
            GetUnaryOperandNames(localVar, out left, out local);
            sb.AppendFormat("{0} = !{1};", local, left);
        }

        private static void HandleXor(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}^{2};", local, left, right);
        }

        private static void HandleOr(BinaryOperator objList, Assignment localVar, StringBuilder sb)
        {
            string right, left, local;
            GetBinaryOperandNames(objList, localVar, out right, out left, out local);

            sb.AppendFormat("{0} = {1}|{2};", local, left, right);
        }

        private static void HandleAnd(BinaryOperator objList, Assignment localVar, StringBuilder sb)
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
    }
}