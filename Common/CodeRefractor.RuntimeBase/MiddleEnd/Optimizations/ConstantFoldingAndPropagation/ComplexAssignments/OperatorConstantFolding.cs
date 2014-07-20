#region Usings

using System.Collections.Generic;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    [Optimization(Category = OptimizationCategories.Constants)]
    public class OperatorConstantFolding : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var intermediateCode = interpreter.MidRepresentation;

            var localOperations = intermediateCode.LocalOperations;
            var binaryOperations = intermediateCode.UseDef.GetOperationsOfKind(OperationKind.BinaryOperator);
            ComputeBinaryOperations(binaryOperations, localOperations);

            var unaryOperations = intermediateCode.UseDef.GetOperationsOfKind(OperationKind.UnaryOperator);
            ComputeUnaryOperations(unaryOperations, localOperations);
        }

        private void ComputeUnaryOperations(int[] unaryOperations, List<LocalOperation> localOperations)
        {
            foreach (var pos in unaryOperations)
            {
                var unaryAssignment = (UnaryOperator) localOperations[pos];

                var constLeft = unaryAssignment.Left as ConstValue;
                if (constLeft == null)
                    continue;

                switch (unaryAssignment.Name)
                {
                    case OpcodeOperatorNames.ConvR8:
                        HandleConvDouble(constLeft, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.ConvR4:
                        HandleConvFloat(constLeft, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Neg:
                        HandleNeg(constLeft, localOperations, pos);
                        break;
                }
            }
        }

        private void ComputeBinaryOperations(int[] binaryOperations, List<LocalOperation> localOperations)
        {
            foreach (var pos in binaryOperations)
            {
                var baseOperator = (OperatorBase) localOperations[pos];

                var unaryAssignment = (BinaryOperator) baseOperator;

                var constLeft = unaryAssignment.Left as ConstValue;
                var constRight = unaryAssignment.Right as ConstValue;
                if (constLeft == null || constRight == null)
                    continue;
                switch (baseOperator.Name)
                {
                    case OpcodeOperatorNames.Add:
                        HandleAdd(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Sub:
                        HandleSub(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Mul:
                        HandleMul(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Div:
                        HandleDiv(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Rem:
                        HandleRem(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Cgt:
                        HandleCgt(constLeft, constRight, localOperations, pos);
                        break;

                    case OpcodeOperatorNames.Clt:
                        HandleClt(constLeft, constRight, localOperations, pos);
                        break;

                    case OpcodeOperatorNames.Ceq:
                        HandleCeq(constLeft, constRight, localOperations, pos);
                        break;

                    case OpcodeOperatorNames.And:
                        HandleAnd(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Or:
                        HandleOr(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Xor:
                        HandleXor(constLeft, constRight, localOperations, pos);
                        break;
                }
            }
        }

        private static List<int> GetBinaryOperations(LocalOperation[] localOperations)
        {
            var binaryOperations = new List<int>();

            for (var index = 0; index < localOperations.Length; index++)
            {
                var destOperation = localOperations[index];
                if (destOperation.Kind != OperationKind.BinaryOperator)
                    continue;
                binaryOperations.Add(index);
            }
            return binaryOperations;
        }

        private static List<int> GetUnaryOperations(LocalOperation[] localOperations)
        {
            var unaryOperations = new List<int>();
            for (var index = 0; index < localOperations.Length; index++)
            {
                var destOperation = localOperations[index];
                if (destOperation.Kind != OperationKind.UnaryOperator)
                    continue;
                unaryOperations.Add(index);
            }
            return unaryOperations;
        }

        private void HandleNeg(ConstValue constLeft, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeNeg(constLeft);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleConvDouble(ConstValue constLeft, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeDouble(constLeft);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleConvFloat(ConstValue constLeft, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeFloat(constLeft);
            FoldConstant(result, localOperations, pos);
        }


        private void HandleCeq(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeCeq(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleClt(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeClt(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleCgt(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeCgt(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        #region Compute math operations

        private void HandleAdd(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeAdd(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleSub(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeSub(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }


        private void HandleMul(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeMul(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleRem(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeRem(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleDiv(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeDiv(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        #endregion

        #region Evaluate bit operations

        private void HandleXor(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeXor(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleAnd(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeAnd(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        private void HandleOr(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeOr(constLeft, constRight);
            FoldConstant(result, localOperations, pos);
        }

        #endregion

        private void FoldConstant(object result, List<LocalOperation> localOperations, int pos)
        {
            var baseOperator = (OperatorBase) localOperations[pos];
            var resultAssignment = new Assignment
            {
                AssignedTo = baseOperator.AssignedTo,
                Right = new ConstValue(result)
            };
            localOperations[pos] =
                resultAssignment;
            Result = true;
        }
    }
}