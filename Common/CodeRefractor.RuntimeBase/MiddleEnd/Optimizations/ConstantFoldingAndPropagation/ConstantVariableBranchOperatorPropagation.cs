#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.Optimizations;
using CodeRefractor.Shared;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation
{
    [Optimization(Category = OptimizationCategories.Constants)]
    public class ConstantVariableBranchOperatorPropagation : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var branchOperations = useDef.GetOperationsOfKind(OperationKind.BranchOperator);
            foreach (var i in branchOperations)
            {
                var destOperation = operations[i];
                var destAssignment = (BranchOperator) destOperation;
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

        static void ApplyChange(CilMethodInterpreter interpreter, ConstValue constValue,
            BranchOperator destAssignment,
            int i)
        {
            var expressionValue = Convert.ToInt32(constValue.Value) != 0;
            var isTrue = (expressionValue) ^ (destAssignment.Name != OpcodeBranchNames.BrTrue);
            var operationList = interpreter.MidRepresentation.LocalOperations;
            if (isTrue)
            {
                operationList[i] = new AlwaysBranch
                {
                    JumpTo = destAssignment.JumpTo
                };
            }
            else
            {
                operationList.RemoveAt(i);
            }
        }
    }
}