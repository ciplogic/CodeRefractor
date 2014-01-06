#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    public class DceVRegUnused : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter intermediateCode)
        {
            var operations = intermediateCode.MidRepresentation.LocalOperations;
            var vregConstants = new HashSet<int>(intermediateCode.MidRepresentation.Vars.VirtRegs.Select(localVar => localVar.Id));
            RemoveCandidatesInDefinitions(operations, vregConstants);
            RemoveCandidatesInUsages(operations, vregConstants);
            if(vregConstants.Count==0)
                return;
            OptimizeUnusedVregs(vregConstants, intermediateCode.MidRepresentation.Vars);
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
                var definition = op.GetDefinition();
                RemoveCandidate(vregConstants, definition);
            }
        }

        private static void RemoveCandidate(HashSet<int> vregConstants, LocalVariable definition)
        {
            if (definition == null)
                return;
            if (definition.Kind != VariableKind.Vreg)
                return;
            vregConstants.Remove(definition.Id);
        }

        private static void OptimizeUnusedVregs(HashSet<int> vregConstants, MidRepresentationVariables variables)
        {
            if (vregConstants.Count == 0)
                return;
            var liveVRegs =
            variables.VirtRegs.Where(
                vreg => vreg.Kind != VariableKind.Vreg || !vregConstants.Contains(vreg.Id)).ToList();
            variables.VirtRegs = liveVRegs;
        }
        #endregion
    }
}