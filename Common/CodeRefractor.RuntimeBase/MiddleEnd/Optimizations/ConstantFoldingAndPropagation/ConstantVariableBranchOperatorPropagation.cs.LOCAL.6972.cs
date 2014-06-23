#region Usings

using System;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.ConstantFoldingAndPropagation
{
    [Optimization(Category = OptimizationCategories.Constants)]
    public class ConstantVariableBranchOperatorPropagation : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var branchOperations = useDef.GetOperationsOfKind(OperationKind.BranchOperator);
            foreach (var i in branchOperations)
            {
                var destOperation = operations[i];
                var destAssignment = (BranchOperator) destOperation.Value;
                if (destAssignment.Name != OpcodeBranchNames.BrTrue && destAssignment.Name != OpcodeBranchNames.BrFalse)
                    continue;
                var constValue = destAssignment.CompareValue as ConstValue;
                if (constValue == null)
                    continue;
                Result = true;
                ApplyChange(interpreter, constValue, destAssignment, i);
                return;
            }
        }

        private static void ApplyChange(MethodInterpreter interpreter, ConstValue constValue, BranchOperator destAssignment,
            int i)
        {
            var expressionValue = Convert.ToInt32(constValue.Value) != 0;
            var isTrue = (expressionValue) ^ (destAssignment.Name != OpcodeBranchNames.BrTrue);
            var operationList = interpreter.MidRepresentation.LocalOperations;
            if (isTrue)
            {
                operationList[i] = new LocalOperation
                {
                    Value = new AlwaysBranch { JumpTo = destAssignment.JumpTo } 
                };
            }
            else
            {
                operationList.RemoveAt(i);
            }
        }
    }
}