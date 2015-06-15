#region Uses

using System;
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
    /// <summary>
    ///     This class reduces operators that are operated with simple constants to an equivalent assignment: a = b*0 => a = 0
    ///     a = b*1 => a = b etc.
    /// </summary>
    [Optimization(Category = OptimizationCategories.Constants)]
    public class OperatorPartialConstantFolding : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var localOperations = interpreter.MidRepresentation.LocalOperations.ToArray();
            for (var index = 0; index < localOperations.Length; index++)
            {
                var destOperation = localOperations[index];
                if (destOperation.Kind != OperationKind.BinaryOperator)
                    continue;

                var destAssignment = (OperatorBase) destOperation;
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
                        HandleMul(constLeft, constRight, destOperation, localOperations, index);
                        break;
                    case OpcodeOperatorNames.Div:
                        HandleDiv(constLeft, constRight, destOperation, localOperations, index);
                        break;
                }
            }
        }

        void FoldAssign(IdentifierValue constResult, LocalOperation[] localOperations, int pos)
        {
            localOperations[pos] = new Assignment { Right = constResult };
        }

        void HandleMul(ConstValue constLeft, ConstValue constRight,
            LocalOperation destOperation, LocalOperation[] localOperations, int pos)
        {
            var binaryOperator = (BinaryOperator)destOperation;

            if (constRight != null && (int)constRight.Value == 1)
            {
                FoldAssign(binaryOperator.Left, localOperations, pos);
                Result = true;
                return;
            }
            var constValue = constLeft ?? constRight;
            if (constValue != null && constValue.Value is int && (int)constValue.Value == 0)
            {
                FoldAssign(constValue, localOperations, pos);
                Result = true;
            }
            if (constLeft != null && constLeft.Value is double && (double)constValue.Value == 0.0)
            {
                FoldAssign(constValue, localOperations, pos);
                Result = true;
                return;
            }
            if (constLeft != null && constValue.Value is float && (float)constValue.Value == 0.0)
            {
                FoldAssign(constValue, localOperations, pos);
                Result = true;
            }
        }

        void HandleDiv(ConstValue constLeft, ConstValue constRight,
            LocalOperation destOperation, LocalOperation[] localOperations, int pos)
        {
            var binaryOperator = (BinaryOperator)destOperation;

            if (constRight != null && constRight.Value is int && (int)constRight.Value == 1)
            {
                throw new NotImplementedException();
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is int && (int)constLeft.Value == 0)
            {
                FoldAssign(binaryOperator.Left, localOperations, pos);
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is double && (double)constLeft.Value == 0.0)
            {
                FoldAssign(binaryOperator.Left, localOperations, pos);
                Result = true;
                return;
            }
            if (constLeft != null && constLeft.Value is float && (float)constLeft.Value == 0.0)
            {
                FoldAssign(binaryOperator.Left, localOperations, pos);
                Result = true;
            }
        }
    }
}