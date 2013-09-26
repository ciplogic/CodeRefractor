using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class PropagationVariablesOptimizationPass : BlockOptimizationPass
    {
        Dictionary<LocalVariable, IdentifierValue> _dictionary = new Dictionary<LocalVariable, IdentifierValue>(); 
        public override bool OptimizeBlock(List<LocalOperation> localOperations, int startRange, int endRange)
        {
            var result = false;
            _dictionary.Clear();
            for(var i = startRange; i<=endRange; i++)
            {
                var op = localOperations[i];
                foreach(var possibleUsage in _dictionary)
                {
                    if (!op.OperationUses(possibleUsage.Key)) continue;
                    op.SwitchUsageWithDefinition(possibleUsage.Key, possibleUsage.Value);
                    result = true;
                }

                var isAssign = op.Kind == OperationKind.Assignment;
                if (!isAssign)
                {
                    var definition = op.GetUseDefinition();
                    if (definition!=null && _dictionary.ContainsKey(definition))
                    {
                        _dictionary.Remove(definition);
                    }
                    continue;
                }
                var assignment = op.GetAssignment();
                _dictionary[assignment.AssignedTo] = assignment.Right;
                var rightAssignment = assignment.Right as LocalVariable;
                if (rightAssignment == null) continue;
                var toRemove = new HashSet<LocalVariable>();
                foreach (var identifierValue in _dictionary.Where(identifierValue => identifierValue.Value.Equals(rightAssignment)))
                {
                    toRemove.Add(identifierValue.Key);
                }
                if (toRemove.Count <= 0) continue;
                foreach (var variable in toRemove)
                {
                    _dictionary.Remove(variable);
                }
            }
            return result;
        }
    }
}