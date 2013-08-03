using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.Compiler.Optimizations
{
    /// <summary>
    /// This class reduces operators that are operated with simple constants to
    /// an equivalent assignment:
    /// a = b*0 => a = 0
    /// a = b*1 => a = b
    /// etc.
    /// </summary>
    public class OperatorPartialConstantFolding : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            foreach (var destOperation in intermediateCode.LocalOperations)
            {
                if (destOperation.Kind != LocalOperation.Kinds.BinaryOperator)
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
                    if (constLeft == null && constRight == null)
                        continue;
                }
                switch (baseOperator.Name)
                {
                    case OpcodeOperatorNames.Mul:
                        HandleMul(constLeft, constRight, destAssignment, destOperation);
                        break;
                    case OpcodeOperatorNames.Div:
                        HandleDiv(constLeft, constRight, destAssignment, destOperation);
                        break;
                }
            }
        }


        private void HandleMul(ConstValue constLeft, ConstValue constRight, Assignment destAssignment,
                               LocalOperation destOperation)
        {
            var binaryOperator = (BinaryOperator) destAssignment.Right;

            if (constRight != null && (int) constRight.Value == 1)
            {
                destAssignment.Right = binaryOperator.Left;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
                return;
            }
            var constValue = constLeft ?? constRight;
            if (constValue != null && constValue.Value is int && (int) constValue.Value == 0)
            {
                destAssignment.Right = constValue;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
            }
            if (constLeft != null && constLeft.Value is double && (double) constValue.Value == 0.0)
            {
                destAssignment.Right = constValue;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
                return;
            }
            if (constLeft != null && constValue.Value is float && (float) constValue.Value == 0.0)
            {
                destAssignment.Right = constValue;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
                return;
            }
        }

        private void HandleDiv(ConstValue constLeft, ConstValue constRight, Assignment destAssignment,
                               LocalOperation destOperation)
        {
            var binaryOperator = (BinaryOperator) destAssignment.Right;

            if (constRight != null && (int) constRight.Value == 1)
            {
                destAssignment.Right = binaryOperator.Left;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is int && (int) constLeft.Value == 0)
            {
                destAssignment.Right = binaryOperator.Left;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is double && (double) constLeft.Value == 0.0)
            {
                destAssignment.Right = binaryOperator.Left;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is float && (float) constLeft.Value == 0.0)
            {
                destAssignment.Right = binaryOperator.Left;
                destOperation.Kind = LocalOperation.Kinds.Assignment;
                Result = true;
                return;
            }
        }
    }
}