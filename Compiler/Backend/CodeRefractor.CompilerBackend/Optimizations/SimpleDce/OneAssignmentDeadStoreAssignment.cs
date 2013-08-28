using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    class OneAssignmentDeadStoreAssignment : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var pos = -1;
            var toRemove = new HashSet<int>();
            var localOperations = intermediateCode.LocalOperations;
            foreach (var op in localOperations)
            {
                pos++;

                var opKind = op.Kind;
                if (opKind != LocalOperation.Kinds.Assignment)
                    continue;
                var variableDefinition = op.GetUseDefinition();
                if (variableDefinition == null)
                    continue;

                var usagePos = localOperations.GetVariableUsages(variableDefinition);
                if (usagePos.Count != 1)
                    continue;
                var assignment = op.GetAssignment();
                localOperations[usagePos.First()].SwitchUsageWithDefinition(assignment.AssignedTo, assignment.Right);
                toRemove.Add(pos);
            }
            if (toRemove.Count == 0)
                return;
            intermediateCode.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}