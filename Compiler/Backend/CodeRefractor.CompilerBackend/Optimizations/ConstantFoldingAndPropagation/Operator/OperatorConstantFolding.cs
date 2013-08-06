#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.Operator
{
    public class OperatorConstantFolding : ResultingOptimizationPass
    {
        private MetaMidRepresentation _intermediateCode;
        private OperatorBase _baseOperator;
        private int _pos;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            _intermediateCode = intermediateCode;
            var pos = -1;
            foreach (var destOperation in intermediateCode.LocalOperations)
            {
                pos++;
                _pos = pos;
                if (destOperation.Kind != LocalOperation.Kinds.BinaryOperator
                    && destOperation.Kind != LocalOperation.Kinds.UnaryOperator)
                    continue;

                //var pos = (Assignment) destOperation.Value;
                var baseOperator = (OperatorBase)destOperation.Value;
                _baseOperator = baseOperator;
                ConstValue constLeft = null;
                ConstValue constRight = null;

                var rightBinaryAssignment = baseOperator as BinaryOperator;
                if (rightBinaryAssignment != null)
                {
                    constLeft = rightBinaryAssignment.Left as ConstValue;
                    constRight = rightBinaryAssignment.Right as ConstValue;
                    if (constLeft == null || constRight == null)
                        continue;
                }
                var unaryAssignment = baseOperator as UnaryOperator;
                if (unaryAssignment != null)
                {
                    constLeft = unaryAssignment.Left as ConstValue;
                    if (constLeft == null)
                        continue;
                }
                switch (baseOperator.Name)
                {
                    case OpcodeOperatorNames.Add:
                        HandleAdd(constLeft, constRight);
                        break;
                    case OpcodeOperatorNames.Sub:
                        HandleSub(constLeft, constRight);
                        break;
                    case OpcodeOperatorNames.Mul:
                        HandleMul(constLeft, constRight);
                        break;
                    case OpcodeOperatorNames.Div:
                        HandleDiv(constLeft, constRight);
                        break;
                    case OpcodeOperatorNames.Rem:
                        HandleRem(constLeft, constRight);
                        break;
                    case OpcodeOperatorNames.Cgt:
                        HandleCgt(constLeft, constRight);
                        break;

                    case OpcodeOperatorNames.Clt:
                        HandleClt(constLeft, constRight);
                        break;

                    case OpcodeOperatorNames.Ceq:
                        HandleCeq(constLeft, constRight);
                        break;
                }
            }
        }

        private void HandleCeq(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeCeq(constLeft, constRight);
            FoldConstant(result);
        }

        private void HandleClt(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeClt(constLeft, constRight);
            FoldConstant(result);
        }

        private void HandleCgt(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeCgt(constLeft, constRight);
            FoldConstant(result);
        }


        private void HandleAdd(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeAdd(constLeft, constRight);
            FoldConstant(result);
        }

        private void HandleSub(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeSub(constLeft, constRight);
            FoldConstant(result);
        }


        private void HandleMul(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeMul(constLeft, constRight);
            FoldConstant(result);
        }

        private void HandleRem(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeRem(constLeft, constRight);
            FoldConstant(result);
        }

        private void FoldConstant(object result)
        {
            var resultAssignment = new Assignment
                                       {
                                                   Left = _baseOperator.AssignedTo,
                                                   Right = new ConstValue(result)
                                               };
            _intermediateCode.LocalOperations[_pos] = 
                new LocalOperation
                    {
                        Kind = LocalOperation.Kinds.Assignment,
                        Value = resultAssignment
                };
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

        private void HandleDiv(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeDiv(constLeft, constRight);
            FoldConstant(result);
        }
    }
}