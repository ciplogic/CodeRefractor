using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class PropagationVariablesOptimizationPass : BlockOptimizationPass
    {
        readonly Dictionary<LocalVariable, ConstValue> _constValues
            = new Dictionary<LocalVariable, ConstValue>();
        readonly Dictionary<LocalVariable, LocalVariable> _mappedValues
            = new Dictionary<LocalVariable, LocalVariable>();
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange)
        {
            var result = false;

            var instructionRange = GetInstructionRange(midRepresentation, startRange, endRange);
            _constValues.Clear();
            _mappedValues.Clear();
            foreach (var op in instructionRange)
            {
                result |= UpdateKnownUsages(op);
              
                RemoveDefinitionsIfTheUsageIsInvalidated(op.GetDefinition());

                UpdateInstructionMapping(op);
            }
            return result;
        }

        private void UpdateInstructionMapping(LocalOperation op)
        {
            if (op.Kind != OperationKind.Assignment)
                return;
            var assignment = op.GetAssignment();

            var right = assignment.Right;
            var value = right as ConstValue;
            if (value != null)
            {
                _constValues[assignment.AssignedTo] = value;
            }
            else
            {
                _mappedValues[assignment.AssignedTo] = (LocalVariable) right;
            }
        }

        /// <summary>
        /// This code has to run for the following sample:
        /// a = 3
        /// b = a
        /// a = 5 //here a is not safe to be used for future usages of b
        /// c = b
        /// </summary>
        /// <param name="usageVariable"></param>
        private void RemoveDefinitionsIfTheUsageIsInvalidated(LocalVariable usageVariable)
        {
            if(usageVariable==null)
                return;
            var toRemove = new HashSet<LocalVariable>();
            foreach (var identifierValue in _mappedValues)
            {
                if (!identifierValue.Value.Equals(usageVariable)) continue;
                toRemove.Add(identifierValue.Key);
            }
            if(toRemove.Count==0)
                return;
            foreach (var variable in toRemove)
            {
                _mappedValues.Remove(variable);
                _constValues.Remove(variable);
            }
        }

        private bool UpdateKnownUsages(LocalOperation op)
        {
            if (_mappedValues.Count == 0 && _constValues.Count==0)
                return false;
            var result =false;
            foreach (var possibleUsage in _mappedValues)
            {
                if (!op.OperationUses(possibleUsage.Key)) continue;
                op.SwitchUsageWithDefinition(possibleUsage.Key, possibleUsage.Value);
                result = true;
            }
            foreach (var possibleUsage in _constValues)
            {
                if (!op.OperationUses(possibleUsage.Key)) continue;
                op.SwitchUsageWithDefinition(possibleUsage.Key, possibleUsage.Value);
                result = true;
            }
            return result;
        }
    }
}