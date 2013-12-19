using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
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

            var assignToConstOperations = GetAssignToConstOperations(localOperations);

            if(assignToConstOperations.Count==0)
                return;
            var definedOnce = ComputeDefinedOnce(assignToConstOperations);

            if(definedOnce.Count == 0)
                return;

            FindMultipleDefinedVariables(intermediateCode, definedOnce);

            if (definedOnce.Count == 0)
                return;
            RemoveMultipleDefinedVariables(assignToConstOperations, definedOnce);
            if(assignToConstOperations.Count==0)
                return;
            var instructionsToRemove = new HashSet<int>();
            foreach (var operation in assignToConstOperations)
            {
                instructionsToRemove.Add(operation.Key);
            }
            var valuesMapping = new Dictionary<LocalVariable, ConstValue>();
            foreach (var operation in assignToConstOperations)
            {
                var assign = operation.Value.GetAssignment();
                valuesMapping[assign.AssignedTo] = (ConstValue) assign.Right;
            }
            intermediateCode.DeleteInstructions(instructionsToRemove);

            localOperations = intermediateCode.LocalOperations;
            foreach (var op in localOperations)
            {
                var variableUsages = op.GetUsages();
                if(variableUsages.Count == 0)
                    continue;

                foreach (var usagePos in valuesMapping)
                {
                    op.SwitchUsageWithDefinition(usagePos.Key, usagePos.Value);
                }    
                Result = true;
                return;
            }
            
        }

        private static void RemoveMultipleDefinedVariables(Dictionary<int, LocalOperation> assignToConstOperations, HashSet<LocalVariable> definedOnce)
        {
            var toRemove = new HashSet<int>();
            foreach (var operation in assignToConstOperations)
            {
                var op = operation.Value;
                var assign = op.GetAssignment();
                if (!definedOnce.Contains(assign.AssignedTo))
                    toRemove.Add(operation.Key);
            }
            foreach (var index in toRemove)
            {
                assignToConstOperations.Remove(index);
            }
        }

        private static void FindMultipleDefinedVariables(MetaMidRepresentation intermediateCode, HashSet<LocalVariable> definedOnce)
        {
            var toRemove = new HashSet<LocalVariable>();
            foreach (var variable in definedOnce)
            {
                var definitions = intermediateCode.GetVariableDefinitions(variable);
                if (definitions.Count > 1)
                {
                    toRemove.Add(variable);
                }
            }
            foreach (var variable in toRemove)
            {
                definedOnce.Remove(variable);
            }
        }

        private static HashSet<LocalVariable> ComputeDefinedOnce(Dictionary<int, LocalOperation> assignToConstOperations)
        {
            var definedOnce = new HashSet<LocalVariable>();
            var definedMany = new HashSet<LocalVariable>();
            foreach (var op in assignToConstOperations)
            {
                var left = op.Value.GetAssignment().AssignedTo;
                if (definedOnce.Contains(left))
                {
                    definedMany.Add(left);
                }
                else
                {
                    definedOnce.Add(left);
                }
            }
            foreach (var many in definedMany)
            {
                definedOnce.Remove(many);
            }
            return definedOnce;
        }

        private static Dictionary<int, LocalOperation> GetAssignToConstOperations(List<LocalOperation> localOperations)
        {
            var assignToConstOperations = new Dictionary<int, LocalOperation>();
            for (int index = 0; index < localOperations.Count; index++)
            {
                var op = localOperations[index];
                var opKind = op.Kind;
                if (opKind == OperationKind.Assignment)
                {
                    var assign = op.GetAssignment();
                    if (!(assign.Right is ConstValue))
                        continue;
                    assignToConstOperations[index] = op;
                }
            }
            return assignToConstOperations;
        }
    }
}