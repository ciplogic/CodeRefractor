#region Usings

using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.Compiler.Optimizations
{
    public class OperatorConstantFolding : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            foreach (var destOperation in intermediateCode.LocalOperations)
            {
                if (destOperation.Kind != LocalOperation.Kinds.BinaryOperator
                    && destOperation.Kind != LocalOperation.Kinds.UnaryOperator)
                    continue;

                var destAssignment = (Assignment) destOperation.Value;
                var baseOperator = (Operator) destAssignment.Right;
                ConstValue constLeft = null;
                ConstValue constRight = null;

                var rightBinaryAssignment = destAssignment.Right as BinaryOperator;
                if (rightBinaryAssignment != null)
                {
                    constLeft = rightBinaryAssignment.Left as ConstValue;
                    constRight = rightBinaryAssignment.Right as ConstValue;
                    if (constLeft == null || constRight == null)
                        continue;
                }
                var unaryAssignment = destAssignment.Right as UnaryOperator;
                if (unaryAssignment != null)
                {
                    constLeft = unaryAssignment.Left as ConstValue;
                    if (constLeft == null)
                        continue;
                }
                switch (baseOperator.Name)
                {
                    case OpcodeOperatorNames.Add:
                        HandleAdd(destOperation, constLeft, constRight, destAssignment);
                        break;
                    case OpcodeOperatorNames.Sub:
                        HandleSub(destOperation, constLeft, constRight, destAssignment);
                        break;
                    case OpcodeOperatorNames.Mul:
                        HandleMul(destOperation, constLeft, constRight, destAssignment);
                        break;
                    case OpcodeOperatorNames.Div:
                        HandleDiv(destOperation, constLeft, constRight, destAssignment);
                        break;
                    case OpcodeOperatorNames.Rem:
                        HandleRem(destOperation, constLeft, constRight, destAssignment);
                        break;
                    case OpcodeOperatorNames.Cgt:
                        HandleCgt(destOperation, constLeft, constRight, destAssignment);
                        break;

                    case OpcodeOperatorNames.Clt:
                        HandleClt(destOperation, constLeft, constRight, destAssignment);
                        break;

                    case OpcodeOperatorNames.Ceq:
                        HandleCeq(destOperation, constLeft, constRight, destAssignment);
                        break;
                }
            }
        }

        private void HandleCeq(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeCeq(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }

        private void HandleClt(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeClt(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }

        private void HandleCgt(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeCgt(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }


        private void HandleAdd(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeAdd(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }

        private void HandleSub(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeSub(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }


        private void HandleMul(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeMul(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }

        private void HandleRem(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeRem(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }

        private void FoldConstant(LocalOperation destOperation, Assignment destAssignment, object result)
        {
            destOperation.Kind = LocalOperation.Kinds.Assignment;
            destAssignment.Right = new ConstValue(result);
            Result = true;
        }

        private static object ComputeCeq(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value == (int) constRight.Value ? 1 : 0;
        }

        private static object ComputeCgt(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value > (int) constRight.Value ? 1 : 0;
        }

        private static object ComputeClt(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value < (int) constRight.Value ? 1 : 0;
        }

        private static object ComputeAdd(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value + (int) constRight.Value;
        }

        private static object ComputeSub(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value - (int) constRight.Value;
        }

        private static object ComputeMul(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value*(int) constRight.Value;
        }

        private static object ComputeDiv(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value/(int) constRight.Value;
        }

        private static object ComputeRem(ConstValue constLeft, ConstValue constRight)
        {
            return (int) constLeft.Value%(int) constRight.Value;
        }

        private void HandleDiv(LocalOperation destOperation, ConstValue constLeft, ConstValue constRight,
                               Assignment destAssignment)
        {
            var result = ComputeDiv(constLeft, constRight);
            FoldConstant(destOperation, destAssignment, result);
        }
    }
}