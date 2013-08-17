using System;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

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
                
                if (variableDefinition.Kind != VariableKind.Vreg)
                    continue;

                var destOperation = localOperations[pos];
                if (!destOperation.OperationUses(variableDefinition))
                    continue;
                if(!(assignment.Right is LocalVariable)&&!(assignment.Right is ConstValue))
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