#region Uses

using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.SimpleDce
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
class DeadStoreAssignment : OptimizationPassBase
    {
        static readonly List<OperationKind> NoSideEffectsOperationKinds = new List<OperationKind>
        {
            OperationKind.Assignment,
            OperationKind.BinaryOperator,
            OperationKind.NewArray,
            OperationKind.NewObject,
            OperationKind.GetArrayItem,
            OperationKind.BinaryOperator,
            OperationKind.GetField,
            OperationKind.UnaryOperator
        };

        public DeadStoreAssignment()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var localOperations = useDef.GetLocalOperations();

            var definitions = new Dictionary<LocalVariable, int>();
            ComputeDefinitions(localOperations, definitions);
            RemoveUsages(localOperations, useDef, definitions);
            if (definitions.Count == 0)
                return false;
            var toRemove = BuildRemoveInstructions(localOperations, definitions);
            if (toRemove.Count == 0)
                return false;
            interpreter.MidRepresentation.DeleteInstructions(toRemove);
            return true;
        }

        static void RemoveUsages(LocalOperation[] localOperations, UseDefDescription useDef,
            Dictionary<LocalVariable, int> definitions)
        {
            for (var index = 0; index < localOperations.Length; index++)
            {
                var usages = useDef.GetUsages(index);
                if (definitions.Count == 0)
                    return;
                foreach (var localVariable in usages)
                {
                    definitions.Remove(localVariable);
                }
            }
        }

        void ComputeDefinitions(LocalOperation[] localOperations, Dictionary<LocalVariable, int> definitions)
        {
            for (var index = 0; index < localOperations.Length; index++)
            {
                var op = localOperations[index];
                var variableDefinition = op.GetDefinition();
                if (variableDefinition == null)
                    continue;
                definitions[variableDefinition] = index;
            }
        }

        List<int> BuildRemoveInstructions(LocalOperation[] localOperations,
            Dictionary<LocalVariable, int> definitions)
        {
            var toRemove = new List<int>();
            foreach (var definition in definitions)
            {
                var index = definition.Value;
                var op = localOperations[index];
                var opKind = op.Kind;
                if (NoSideEffectsOperationKinds.Contains(opKind))
                {
                    toRemove.Add(index);
                }
            }
            return toRemove;
        }
    }
}