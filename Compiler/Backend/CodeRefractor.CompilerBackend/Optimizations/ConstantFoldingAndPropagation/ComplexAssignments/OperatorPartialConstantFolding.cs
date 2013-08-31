#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    /// <summary>
    ///   This class reduces operators that are operated with simple constants to an equivalent assignment: a = b*0 => a = 0 a = b*1 => a = b etc.
    /// </summary>
    public class OperatorPartialConstantFolding : ResultingInFunctionOptimizationPass
    {
        private List<LocalOperation> _localOperations;
        private int _pos;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            _localOperations = intermediateCode.LocalOperations;
            _pos = 0;
            foreach (var destOperation in _localOperations)
            {
                _pos++;
                if (destOperation.Kind != LocalOperation.Kinds.BinaryOperator)
                    continue;

                var destAssignment = (OperatorBase)destOperation.Value;
                var baseOperator = destAssignment;
                ConstValue constLeft = null;
                ConstValue constRight = null;

                var rightBinaryAssignment = destAssignment as BinaryOperator;
                if (rightBinaryAssignment != null)
                {
                    constLeft = rightBinaryAssignment.Left as ConstValue;
                    constRight = rightBinaryAssignment.Right as ConstValue;
                    if (constLeft == null && constRight == null)
                        continue;
                }
                switch (baseOperator.Name)
                {
                    case OpcodeOperatorNames.Mul:
                        HandleMul(constLeft, constRight, destOperation);
                        break;
                    case OpcodeOperatorNames.Div:
                        HandleDiv(constLeft, constRight, destOperation);
                        break;
                }
            }
        }

        void FoldAssign(IdentifierValue constResult)
        {
            _localOperations[_pos] = new LocalOperation()
            {
                Kind = LocalOperation.Kinds.Assignment,
                Value = constResult
            };
        }

        private void HandleMul(ConstValue constLeft, ConstValue constRight,
                               LocalOperation destOperation)
        {
            var binaryOperator = (BinaryOperator)destOperation.Value;

            if (constRight != null && (int)constRight.Value == 1)
            {
                FoldAssign(binaryOperator.Left);
                Result = true;
                return;
            }
            var constValue = constLeft ?? constRight;
            if (constValue != null && constValue.Value is int && (int)constValue.Value == 0)
            {
                FoldAssign(constValue);
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
            }
            if (constLeft != null && constLeft.Value is double && (double)constValue.Value == 0.0)
            {
                FoldAssign(constValue);
                Result = true;
                return;
            }
            if (constLeft != null && constValue.Value is float && (float)constValue.Value == 0.0)
            {
                FoldAssign(constValue);
                Result = true;
                return;
            }
        }

        private void HandleDiv(ConstValue constLeft, ConstValue constRight,
                               LocalOperation destOperation)
        {
            var binaryOperator = (BinaryOperator)destOperation.Value;

            if (constRight != null && constRight.Value is int && (int)constRight.Value == 1)
            {
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is int && (int)constLeft.Value == 0)
            {
                FoldAssign(binaryOperator.Left);
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is double && (double)constLeft.Value == 0.0)
            {
                FoldAssign(binaryOperator.Left);
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is float && (float)constLeft.Value == 0.0)
            {
                FoldAssign(binaryOperator.Left);
                Result = true;
                return;
            }
        }
    }
}