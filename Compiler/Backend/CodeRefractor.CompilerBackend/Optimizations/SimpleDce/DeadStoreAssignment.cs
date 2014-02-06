using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{

    class DeadStoreAssignment : ResultingInFunctionOptimizationPass
    {
        static readonly List<OperationKind> NoSideEffectsOperationKinds =new List<OperationKind>
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
        
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            Dictionary<LocalVariable, int> definitions = new Dictionary<LocalVariable, int>();

            var localOperations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();

            var useDef = methodInterpreter.MidRepresentation.UseDef;
            definitions.Clear();
            ComputeDefinitions(localOperations, definitions);
            RemoveUsages(localOperations, useDef, definitions);
            if (definitions.Count == 0)
                return;
            var toRemove = BuildRemoveInstructions(localOperations, definitions);
            if (toRemove.Count == 0)
                return;
            methodInterpreter.MidRepresentation.DeleteInstructions(toRemove);
            toRemove.Clear();
            Result = true;
        }

        private void RemoveUsages(LocalOperation[] localOperations, UseDefDescription useDef, Dictionary<LocalVariable, int> definitions)
        {
            for (int index = 0; index < localOperations.Length; index++)
            {
                var usages = useDef.GetUsages(index);
                foreach (var localVariable in usages)
                {
                    definitions.Remove(localVariable);
                }
            }
        }

        private void ComputeDefinitions(LocalOperation[] localOperations, Dictionary<LocalVariable, int> definitions)
        {
            for (int index = 0; index < localOperations.Length; index++)
            {
                var op = localOperations[index];
                var variableDefinition = op.GetDefinition();
                if (variableDefinition == null)
                    continue;
                definitions[variableDefinition] = index;
            }
        }

        private List<int> BuildRemoveInstructions(LocalOperation[] localOperations, Dictionary<LocalVariable, int> definitions)
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
