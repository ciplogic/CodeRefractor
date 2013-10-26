using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    class DeadStoreAssignment : ResultingInFunctionOptimizationPass
    {
        readonly Dictionary<LocalVariable, int> _definitions = new Dictionary<LocalVariable, int>();
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var toRemove = new HashSet<int>();
            _definitions.Clear();
            var localOperations = intermediateCode.LocalOperations;

            for (int index = 0; index < localOperations.Count; index++)
            {
                var op = localOperations[index];
                var variableDefinition = op.GetUseDefinition();
                if (variableDefinition == null)
                    continue;
                _definitions[variableDefinition] = index;
            }

            foreach (var op in localOperations)
            {
                var usages = op.GetUsages();
                foreach (var localVariable in usages)
                {
                    _definitions.Remove(localVariable);
                }
            }
            if (_definitions.Count == 0)
                return;

            for (var index = 0; index < localOperations.Count; index++)
            {
                var op = localOperations[index];
                var opKind = op.Kind;
                if (opKind != OperationKind.Assignment
                   && opKind != OperationKind.BinaryOperator
                   && opKind != OperationKind.NewArray
                   && opKind != OperationKind.NewObject
                   && opKind != OperationKind.GetArrayItem
                   && opKind != OperationKind.BinaryOperator
                   && opKind != OperationKind.GetField
                    && opKind != OperationKind.UnaryOperator)
                    continue;
                var variableDefinition = op.GetUseDefinition();
                if (_definitions.ContainsKey(variableDefinition))
                    toRemove.Add(index);
            }
            if (toRemove.Count == 0)
                return;
            intermediateCode.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}
