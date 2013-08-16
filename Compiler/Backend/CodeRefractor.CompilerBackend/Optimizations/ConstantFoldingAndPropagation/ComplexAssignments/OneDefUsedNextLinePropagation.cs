using System;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class OneDefUsedNextLinePropagation : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var pos = 0;
            var localOperations = intermediateCode.LocalOperations;
            foreach (var op in localOperations)
            {
                pos++;
                var assignment = op.GetAssignment();
                if(assignment==null)
                    continue;
                var variableDefinition = op.GetUseDefinition();
                if(variableDefinition==null)
                    continue;

                var destOperation = localOperations[pos];
                if (!destOperation.OperationUses(variableDefinition))
                    continue;
                try
                {
                    destOperation.SwitchUsageWithDefinition(variableDefinition, assignment.Right);
                    Result = true;
                    return;
                }
                catch (Exception)
                {
                    continue;
                }

            }
            
        }
    }

    class OneDefUsedPreviousLinePropagation : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var pos = -1;
            var localOperations = intermediateCode.LocalOperations;
            foreach (var op in localOperations)
            {
                pos++;
                if(pos==0)
                    continue;
                var assignment = op.GetAssignment();
                if (assignment == null)
                    continue;
                var variableDefinition = op.GetUseDefinition();
                if (variableDefinition == null)
                    continue;

                var destOperation = localOperations[pos];
                if (!destOperation.OperationUses(variableDefinition))
                    continue;
                try
                {
                    destOperation.SwitchUsageWithDefinition(variableDefinition, assignment.Right);
                    Result = true;
                    return;
                }
                catch (Exception)
                {
                    continue;
                }

            }

        }
    }
}