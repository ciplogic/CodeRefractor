#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase;
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
        private int _pos;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            _intermediateCode = intermediateCode;
            var localOperations = intermediateCode.LocalOperations;
            var binaryOperations = GetBinaryOperations(localOperations);
            ComputeBinaryOperations(binaryOperations);

            var unaryOperations = GetUnaryOperations(localOperations);
            ComputeUnaryOperations(unaryOperations);
        }

        private void ComputeUnaryOperations(Dictionary<int, LocalOperation> unaryOperations)
        {
            foreach (var unaryOperation in unaryOperations)
            {
                var baseOperator = (OperatorBase) unaryOperation.Value.Value;

                var unaryAssignment = (UnaryOperator) baseOperator;

                var constLeft = unaryAssignment.Left as ConstValue;
                if (constLeft == null)
                    continue;

                _pos = unaryOperation.Key;
                switch (baseOperator.Name)
                {
                    case OpcodeOperatorNames.ConvR8:
                        HandleConvDouble(constLeft);
                        break;
                    case OpcodeOperatorNames.ConvR4:
                        HandleConvFloat(constLeft);
                        break;
                }
            }
        }

        private void ComputeBinaryOperations(Dictionary<int, LocalOperation> binaryOperations)
        {
            foreach (var binaryOperation in binaryOperations)
            {
                var baseOperator = (OperatorBase) binaryOperation.Value.Value;

                var unaryAssignment = (BinaryOperator) baseOperator;

                var constLeft = unaryAssignment.Left as ConstValue;
                var constRight = unaryAssignment.Right as ConstValue;
                if (constLeft == null || constRight == null)
                    continue;
                _pos = binaryOperation.Key;
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
                }
            }
        }

        private static Dictionary<int, LocalOperation> GetBinaryOperations(List<LocalOperation> localOperations)
        {
            var binaryOperations = new Dictionary<int, LocalOperation>();

            for (var index = 0; index < localOperations.Count; index++)
            {
                var destOperation = localOperations[index];
                if (destOperation.Kind != OperationKind.BinaryOperator)
                    continue;
                binaryOperations[index] = destOperation;
            }
            return binaryOperations;
        }

        private static Dictionary<int, LocalOperation> GetUnaryOperations(List<LocalOperation> localOperations)
        {
            var unaryOperations = new Dictionary<int, LocalOperation>();
            for (var index = 0; index < localOperations.Count; index++)
            {
                var destOperation = localOperations[index];
                if (destOperation.Kind != OperationKind.UnaryOperator)
                    continue;
                unaryOperations[index] = destOperation;
            }
            return unaryOperations;
        }

        private void HandleNeg(ConstValue constLeft)
        {
            var result = ComputeNeg(constLeft);

            FoldConstant(result);
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
        #region Compute

        private static object ComputeNeg(ConstValue constLeft)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int32:
                    return -(int)constLeft.Value;
                case TypeCode.Double:
                    return -(double)constLeft.Value;
                case TypeCode.Single:
                    return -(float)constLeft.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }


        private static object ComputeAdd(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int32:
                    return (int)constLeft.Value + (int)constRight.Value;
                case TypeCode.Double:
                    return (double)constLeft.Value + (double)constRight.Value;
                case TypeCode.Single:
                    return (float)constLeft.Value + (float)constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        private static object ComputeSub(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int32:
                    return (int)constLeft.Value - (int)constRight.Value;
                case TypeCode.Double:
                    return (double)constLeft.Value - (double)constRight.Value;
                case TypeCode.Single:
                    return (float)constLeft.Value - (float)constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        private static object ComputeMul(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int32:
                    return (int)constLeft.Value * (int)constRight.Value;
                case TypeCode.Double:
                    return (double)constLeft.Value * (double)constRight.Value;
                case TypeCode.Single:
                    return (float)constLeft.Value * (float)constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        private static object ComputeDiv(ConstValue constLeft, ConstValue constRight)
        {
            var typeCode = constLeft.Value.ComputeTypeCode();
            switch (typeCode)
            {
                case TypeCode.Int32:
                    return (int)constLeft.Value / (int)constRight.Value;
                case TypeCode.Double:
                    return (double)constLeft.Value / (double)constRight.Value;
                case TypeCode.Single:
                    return (float)constLeft.Value / (float)constRight.Value;
            }
            throw new InvalidDataException("This type combination is not implemented");
        }

        private static object ComputeRem(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value % (int)constRight.Value;
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
            return (int)constLeft.Value == (int)constRight.Value ? 1 : 0;
        }

        private static object ComputeCgt(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value > (int)constRight.Value ? 1 : 0;
        }

        private static object ComputeClt(ConstValue constLeft, ConstValue constRight)
        {
            return (int)constLeft.Value < (int)constRight.Value ? 1 : 0;
        }
        #endregion
        
        #endregion

        #region Evaluate bit operations


        private void HandleDiv(ConstValue constLeft, ConstValue constRight)
        {
            var result = ComputeDiv(constLeft, constRight);
            FoldConstant(result);
        }

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
        #endregion

        private void FoldConstant(object result)
        {
            var baseOperator = (OperatorBase)_intermediateCode.LocalOperations[_pos].Value;
            var resultAssignment = new Assignment
                                       {
                                                   AssignedTo = baseOperator.AssignedTo,
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

    }
}