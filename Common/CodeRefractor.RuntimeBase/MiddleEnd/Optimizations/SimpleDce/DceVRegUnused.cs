#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.SimpleDce
{	
	[Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class DceVRegUnused : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var operations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var vregConstants =
                new HashSet<int>(interpreter.MidRepresentation.Vars.VirtRegs.Select(localVar => localVar.Id));

            var useDef = interpreter.MidRepresentation.UseDef;
            RemoveCandidatesInDefinitions(operations, vregConstants, useDef);
            RemoveCandidatesInUsages(operations, vregConstants, useDef);
            if (vregConstants.Count == 0)
                return;
            OptimizeUnusedVregs(vregConstants, interpreter.MidRepresentation.Vars);
        }

        #region Remove candidates

        private static void RemoveCandidatesInUsages(LocalOperation[] operations, HashSet<int> vregConstants,
            UseDefDescription useDef)
        {
            for (var index = 0; index < operations.Length; index++)
            {
                var usages = useDef.GetUsages(index);
                foreach (var usage in usages)
                {
                    RemoveCandidate(vregConstants, usage);
                }
            }
        }

        private static void RemoveCandidatesInDefinitions(LocalOperation[] operations, HashSet<int> vregConstants,
            UseDefDescription useDef)
        {
            for (var index = 0; index < operations.Length; index++)
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