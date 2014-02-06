#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    public class DceVRegUnused : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var operations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();
            var vregConstants = new HashSet<int>(methodInterpreter.MidRepresentation.Vars.VirtRegs.Select(localVar => localVar.Id));

            var useDef = methodInterpreter.MidRepresentation.UseDef;
            RemoveCandidatesInDefinitions(operations, vregConstants, useDef);
            RemoveCandidatesInUsages(operations, vregConstants, useDef);
            if(vregConstants.Count==0)
                return;
            OptimizeUnusedVregs(vregConstants, methodInterpreter.MidRepresentation.Vars);
        }

        #region Remove candidates
        private static void RemoveCandidatesInUsages(LocalOperation[] operations, HashSet<int> vregConstants, UseDefDescription useDef)
        {
            for (int index = 0; index < operations.Length; index++)
            {
                var usages = useDef.GetUsages(index);
                foreach (var usage in usages)
                {
                    RemoveCandidate(vregConstants, usage);
                }
            }
        }

        private static void RemoveCandidatesInDefinitions(LocalOperation[] operations, HashSet<int> vregConstants, UseDefDescription useDef)
        {
            for (int index = 0; index < operations.Length; index++)
            {
                var definition = useDef.GetDefinition(index);
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