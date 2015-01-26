#region Usings

using System.Collections.Generic;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.SimpleDce
{	
	[Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class DceLocalAssigned : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var operations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var vregConstants = new HashSet<int>();
            foreach (var localVariable in interpreter.MidRepresentation.Vars.LocalVars)
                vregConstants.Add(localVariable.Id);

            var useDef = interpreter.MidRepresentation.UseDef;
            RemoveCandidatesInDefinitions(operations, vregConstants, useDef);
            RemoveCandidatesInUsages(operations, vregConstants, useDef);
            if (vregConstants.Count == 0)
                return;

            OptimizeUnusedLocals(vregConstants, interpreter.MidRepresentation.Vars);
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