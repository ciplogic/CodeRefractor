#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    public class DceLocalAssigned : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            var vregConstants = new HashSet<int>();
            foreach (var localVariable in intermediateCode.Vars.LocalVars)
                vregConstants.Add(localVariable.Id);

            RemoveCandidatesInDefinitions(operations, vregConstants);
            RemoveCandidatesInUsages(operations, vregConstants);
            if (vregConstants.Count == 0)
                return;
            
            OptimizeUnusedLocals(vregConstants, intermediateCode.Vars);
        }

        #region Remove candidates

        private static void RemoveCandidatesInUsages(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var op in operations)
            {
                var usages = op.GetUsages();
                foreach (var usage in usages)
                {
                    RemoveCandidate(vregConstants, usage);
                }
            }
        }

        private static void RemoveCandidatesInDefinitions(List<LocalOperation> operations, HashSet<int> vregConstants)
        {
            foreach (var op in operations)
            {
                var definition = op.GetUseDefinition();
                RemoveCandidate(vregConstants, definition);
            }
        }

        private static void RemoveCandidate(HashSet<int> locals, LocalVariable definition)
        {
            if (definition == null)
                return;
            if (definition.Kind == VariableKind.Vreg)
                return;
            locals.Remove(definition.Id);
        }

        private static void OptimizeUnusedLocals(HashSet<int> localConstants, MidRepresentationVariables variables)
        {
            if (localConstants.Count == 0)
                return;

            foreach (var localUnused in localConstants)
            {
                variables.LocalVars.RemoveAll(local => local.Id == localUnused);
            }
        }

        #endregion
    }
}