#region Uses

using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    [Optimization(Category = OptimizationCategories.Constants)]
    public class OperatorConstantFolding : OptimizationPassBase
    {
        public OperatorConstantFolding()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var intermediateCode = interpreter.MidRepresentation;

            var localOperations = intermediateCode.LocalOperations;
            var binaryOperations = intermediateCode.UseDef.GetOperationsOfKind(OperationKind.BinaryOperator);
            var result = false;
            result |= ComputeBinaryOperations(binaryOperations, localOperations);

            var unaryOperations = intermediateCode.UseDef.GetOperationsOfKind(OperationKind.UnaryOperator);
            result |= ComputeUnaryOperations(unaryOperations, localOperations);
            return result;
        }

        bool ComputeUnaryOperations(int[] unaryOperations, List<LocalOperation> localOperations)
        {
            var result = false;
            foreach (var pos in unaryOperations)
            {
                var unaryAssignment = (UnaryOperator)localOperations[pos];

                var constLeft = unaryAssignment.Left as ConstValue;
                if (constLeft == null)
                    continue;

                switch (unaryAssignment.Name)
                {
                    case OpcodeOperatorNames.ConvR8:
                        result |= HandleConvDouble(constLeft, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.ConvR4:
                        result |= HandleConvFloat(constLeft, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Neg:
                        result |= HandleNeg(constLeft, localOperations, pos);
                        break;
                }
            }
            return result;
        }

        bool ComputeBinaryOperations(int[] binaryOperations, List<LocalOperation> localOperations)
        {
            var result = false;
            foreach (var pos in binaryOperations)
            {
                var baseOperator = (OperatorBase)localOperations[pos];

                var unaryAssignment = (BinaryOperator)baseOperator;

                var constLeft = unaryAssignment.Left as ConstValue;
                var constRight = unaryAssignment.Right as ConstValue;
                if (constLeft == null || constRight == null)
                    continue;
                switch (baseOperator.Name)
                {
                    case OpcodeOperatorNames.Add:
                        result |= HandleAdd(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Sub:
                        result |= HandleSub(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Mul:
                        result |= HandleMul(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Div:
                        result |= HandleDiv(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Rem:
                        result |= HandleRem(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Cgt:
                        result |= HandleCgt(constLeft, constRight, localOperations, pos);
                        break;

                    case OpcodeOperatorNames.Clt:
                        result |= HandleClt(constLeft, constRight, localOperations, pos);
                        break;

                    case OpcodeOperatorNames.Ceq:
                        result |= HandleCeq(constLeft, constRight, localOperations, pos);
                        break;

                    case OpcodeOperatorNames.And:
                        result |= HandleAnd(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Or:
                        result |= HandleOr(constLeft, constRight, localOperations, pos);
                        break;
                    case OpcodeOperatorNames.Xor:
                        result |= HandleXor(constLeft, constRight, localOperations, pos);
                        break;
                }
            }
            return result;
        }

        bool HandleNeg(ConstValue constLeft, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeNeg(constLeft);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleConvDouble(ConstValue constLeft, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeDouble(constLeft);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleConvFloat(ConstValue constLeft, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeFloat(constLeft);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleCeq(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeCeq(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleClt(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeClt(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleCgt(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeCgt(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool FoldConstant(object result, List<LocalOperation> localOperations, int pos)
        {
            var baseOperator = (OperatorBase)localOperations[pos];
            var resultAssignment = new Assignment
            {
                AssignedTo = baseOperator.AssignedTo,
                Right = new ConstValue(result)
            };
            localOperations[pos] =
                resultAssignment;
            return true;
        }

        #region Compute math operations

        bool HandleAdd(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeAdd(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleSub(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeSub(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }


        bool HandleMul(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeMul(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleRem(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeRem(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleDiv(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeDiv(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        #endregion

        #region Evaluate bit operations

        bool HandleXor(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeXor(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleAnd(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations,
            int pos)
        {
            var result = ComputeConstantOperator.ComputeAnd(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        bool HandleOr(ConstValue constLeft, ConstValue constRight, List<LocalOperation> localOperations, int pos)
        {
            var result = ComputeConstantOperator.ComputeOr(constLeft, constRight);
            return FoldConstant(result, localOperations, pos);
        }

        #endregion
    }
}