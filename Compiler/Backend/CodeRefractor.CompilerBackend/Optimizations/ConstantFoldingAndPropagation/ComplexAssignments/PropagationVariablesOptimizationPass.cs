using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class PropagationVariablesOptimizationPass : BlockOptimizationPass
    {
        readonly Dictionary<LocalVariable, IdentifierValue> _dictionary 
            = new Dictionary<LocalVariable, IdentifierValue>();

        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange)
        {
            var result = false;

            var instructionRange = GetInstructionRange(midRepresentation, startRange, endRange);
            _dictionary.Clear();
            foreach (var op in instructionRange)
            {
                result = UpdateKnownUsages(op);
                var isAssign = op.Kind == OperationKind.Assignment;
                if (!isAssign)
                {
                    var definition = op.GetDefinition();
                    if (definition != null && _dictionary.ContainsKey(definition))
                        _dictionary.Remove(definition);
                    continue;
                }
                var assignment = op.GetAssignment();
                RemoveDefinitionsIfTheUsageIsInvalidated(assignment.AssignedTo);

                _dictionary[assignment.AssignedTo] = assignment.Right;
            }
            return result;
        }

        /// <summary>
        /// This code has to run for the following sample:
        /// a = 3
        /// b = a
        /// a = 5 //here a is not safe to be used for future usages of b
        /// c = b
        /// </summary>
        /// <param name="rightAssignment"></param>
        private void RemoveDefinitionsIfTheUsageIsInvalidated(LocalVariable rightAssignment)
        {
            var toRemove = new HashSet<LocalVariable>();
            foreach (var identifierValue in _dictionary)
            {
                if (!identifierValue.Value.Equals(rightAssignment)) continue;
                toRemove.Add(identifierValue.Key);
            }
            if(toRemove.Count==0)
                return;
            foreach (var variable in toRemove)
            {
                _dictionary.Remove(variable);
            }
        }

        private bool UpdateKnownUsages(LocalOperation op)
        {
            if (_dictionary.Count == 0)
                return false;
            var result =false;
            foreach (var possibleUsage in _dictionary)
            {
                if (op.OperationUses(possibleUsage.Key))
                {
                    op.SwitchUsageWithDefinition(possibleUsage.Key, possibleUsage.Value);
                    result = true;
                }
            }
            return result;
        }
    }
}