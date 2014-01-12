using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class PropagationVariablesOptimizationPass : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange)
        {
            var result = false;

            var instructionRange = GetInstructionRange(midRepresentation, startRange, endRange);
            var constValues = new Dictionary<LocalVariable, ConstValue>();
            var mappedValues = new Dictionary<LocalVariable, LocalVariable>();
            foreach (var op in instructionRange)
            {
                result |= UpdateKnownUsages(op, constValues, mappedValues);
                RemoveDefinitionsIfTheUsageIsInvalidated(op.GetDefinition(), constValues, mappedValues);
                UpdateInstructionMapping(op, constValues, mappedValues);
            }
            return result;
        }

        private static void UpdateInstructionMapping(
            LocalOperation op, 
            Dictionary<LocalVariable, ConstValue> constValues, 
            Dictionary<LocalVariable, LocalVariable> mappedValues)
        {
            if (op.Kind != OperationKind.Assignment)
                return;
            var assignment = op.GetAssignment();

            var right = assignment.Right;
            var value = right as ConstValue;
            if (value != null)
            {
                constValues[assignment.AssignedTo] = value;
            }
            else
            {
                mappedValues[assignment.AssignedTo] = (LocalVariable)right;
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
        /// <param name="constValues"></param>
        /// <param name="mappedValues"></param>
        private static void RemoveDefinitionsIfTheUsageIsInvalidated(
            LocalVariable usageVariable, 
            Dictionary<LocalVariable, ConstValue> constValues, 
            Dictionary<LocalVariable, LocalVariable> mappedValues)
        {
            if(usageVariable==null)
                return;
            var toRemove = new HashSet<LocalVariable>();
            foreach (var identifierValue in mappedValues)
            {
                if (!identifierValue.Value.Equals(usageVariable)) continue;
                toRemove.Add(identifierValue.Key);
            }
            if(toRemove.Count==0)
                return;
            foreach (var variable in toRemove)
            {
                mappedValues.Remove(variable);
                constValues.Remove(variable);
            }
        }

        private static bool UpdateKnownUsages(
            LocalOperation op, 
            Dictionary<LocalVariable, ConstValue> constValues, 
            Dictionary<LocalVariable, LocalVariable> mappedValues)
        {
            if (mappedValues.Count == 0 && constValues.Count == 0)
                return false;
            var result =false;
            foreach (var possibleUsage in mappedValues)
            {
                if (!op.OperationUses(possibleUsage.Key)) continue;
                op.SwitchUsageWithDefinition(possibleUsage.Key, possibleUsage.Value);
                result = true;
            }
            foreach (var possibleUsage in constValues)
            {
                if (!op.OperationUses(possibleUsage.Key)) continue;
                op.SwitchUsageWithDefinition(possibleUsage.Key, possibleUsage.Value);
                result = true;
            }
            return result;
        }
    }
}