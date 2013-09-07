using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    class OneAssignmentDeadStoreAssignment : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOperations = intermediateCode.LocalOperations;
            foreach (var op in localOperations)
            {
                var opKind = op.Kind;
                if (opKind != LocalOperation.Kinds.Assignment)
                    continue;
                var variableDefinition = op.GetUseDefinition();
                if (variableDefinition == null)
                    continue;
                var assignment = op.GetAssignment();
                var canPropagate = (assignment.Right is ConstValue) ||
                                         (((LocalVariable) assignment.Right).Kind != VariableKind.Local);
                if(!canPropagate)
                    continue;
                var defs = intermediateCode.GetVariableDefinitions(variableDefinition);
                if (defs.Count != 1)
                    continue;
                var variableUsages = intermediateCode.GetVariableUsages(variableDefinition);
                
                foreach (var usagePos in variableUsages)
                {
                    localOperations[usagePos].SwitchUsageWithDefinition(assignment.AssignedTo, assignment.Right);
                }    
                Result = true;
                return;
            }
            
        }
    }
}