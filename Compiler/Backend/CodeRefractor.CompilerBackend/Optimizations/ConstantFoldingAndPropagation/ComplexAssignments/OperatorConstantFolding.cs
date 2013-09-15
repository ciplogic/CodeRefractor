#region Usings

using System;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    public class OperatorConstantFolding : ResultingInFunctionOptimizationPass
    {
        private MetaMidRepresentation _intermediateCode;
        private OperatorBase _baseOperator;
        private int _pos;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            _intermediateCode = intermediateCode;
            var pos = -1;
            for (var index = 0; index < intermediateCode.LocalOperations.Count; index++)
            {
                var destOperation = intermediateCode.LocalOperations[index];
                pos++;
                _pos = pos;
                if (destOperation.Kind != OperationKind.BinaryOperator
                    && destOperation.Kind != OperationKind.UnaryOperator)
                    continue;

                var baseOperator = (OperatorBase) destOperation.Value;
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


                    case OpcodeOperatorNames.And:
                        HandleAnd(constLeft, constRight);
                        break;
                    case OpcodeOperatorNames.Or:
                        HandleOr(constLeft, constRight);
                        break;
                    case OpcodeOperatorNames.Xor:
                        HandleXor(constLeft, constRight);
                        break;


                    case OpcodeOperatorNames.ConvR8:
                        HandleConvDouble(constLeft);
                        break;
                    case OpcodeOperatorNames.ConvR4:
                        HandleConvFloat(constLeft);
                        break;
                    default:
                        throw new Exception("cannot evaluate this type");
                }
            }
        }

        private void HandleConvDouble(ConstValue constLeft)
        {
            var result = ComputeDouble(constLeft);
            FoldConstant(result);
        }
        private void HandleConvFloat(ConstValue constLeft)
        {
            var result = ComputeFloat(constLeft);
            FoldConstant(result);
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

        #region Compute math operations
        
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

        private static object ComputeAdd(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value + (int)constRight.Value;
        }

        private static object ComputeSub(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value - (int)constRight.Value;
        }

        private static object ComputeMul(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value * (int)constRight.Value;
        }

        private static object ComputeDiv(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value / (int)constRight.Value;
        }

        private static object ComputeRem(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value % (int)constRight.Value;
        }

        private void HandleDiv(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeDiv(constLeft, constRight);
            FoldConstant(result);
        }
        #endregion

        #region Evaluate bit operations

        private void HandleXor(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeXor(constLeft, constRight);
            FoldConstant(result);
        }

        private void HandleAnd(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeAnd(constLeft, constRight);
            FoldConstant(result);
        }

        private void HandleOr(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeOr(constLeft, constRight);
            FoldConstant(result);
        }

        private static object ComputeOr(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value | (int)constRight.Value;
        }
        private static object ComputeAnd(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value & (int)constRight.Value;
        }

        private static object ComputeXor(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value ^ (int)constRight.Value;
        }
        #endregion

        private void FoldConstant(object result)
        {
            var resultAssignment = new Assignment
                                       {
                                                   AssignedTo = _baseOperator.AssignedTo,
                                                   Right = new ConstValue(result)
                                               };
            _intermediateCode.LocalOperations[_pos] = 
                new LocalOperation
                    {
                        Kind= OperationKind.Assignment,
                        Value = resultAssignment
                };
            Result = true;
        }
        
        private static object ComputeDouble(ConstValue constLeft)
        {
            return Convert.ToDouble(constLeft.Value);
        }

        private static object ComputeFloat(ConstValue constLeft)
        {
            return Convert.ToSingle(constLeft.Value);
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

    }
}