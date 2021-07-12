#region Uses

using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.SimpleDce
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class DceLocalAssigned : OptimizationPassBase
    {
        public DceLocalAssigned()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var operations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var vregConstants = new HashSet<int>();
            foreach (var localVariable in interpreter.MidRepresentation.Vars.LocalVars)
                vregConstants.Add(localVariable.Id);

            var useDef = interpreter.MidRepresentation.UseDef;
            RemoveCandidatesInDefinitions(operations, vregConstants, useDef);
            RemoveCandidatesInUsages(operations, vregConstants, useDef);
            if (vregConstants.Count == 0)
                return false;

            OptimizeUnusedLocals(vregConstants, interpreter.MidRepresentation.Vars);
            return true;
        }

        #region Remove candidates

        static void RemoveCandidatesInUsages(LocalOperation[] operations, HashSet<int> vregConstants,
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

        static void RemoveCandidatesInDefinitions(LocalOperation[] operations, HashSet<int> vregConstants,
            UseDefDescription useDef)
        {
            for (var index = 0; index < operations.Length; index++)
            {
                var definition = useDef.GetDefinition(index);
                RemoveCandidate(vregConstants, definition);
            }
        }

        static void RemoveCandidate(HashSet<int> locals, LocalVariable definition)
        {
            if (definition == null)
                return;
            if (definition.Kind == VariableKind.Vreg)
                return;
            locals.Remove(definition.Id);
        }

        static void OptimizeUnusedLocals(HashSet<int> localConstants, MidRepresentationVariables variables)
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