#region Uses

using System.Collections.Generic;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    [Optimization(Category = OptimizationCategories.Propagation)]
    internal class PropagationVariables : BlockOptimizationPass
    {
        public override bool OptimizeBlock(CilMethodInterpreter midRepresentation, int startRange, int endRange,
            LocalOperation[] operations)
        {
            var instructionRange = GetInstructionRange(operations, startRange, endRange);
            var useDef = midRepresentation.MidRepresentation.UseDef;
            return ProcessOptimizations(startRange, instructionRange, useDef);
        }

        private static bool ProcessOptimizations(int startRange, LocalOperation[] instructionRange,
            UseDefDescription useDef)
        {
            var result = false;
            var constValues = new Dictionary<LocalVariable, ConstValue>();
            var mappedValues = new Dictionary<LocalVariable, LocalVariable>();
            for (var index = 0; index < instructionRange.Length; index++)
            {
                var op = instructionRange[index];
                result |= UpdateKnownUsages(op, constValues, mappedValues, useDef, startRange + index);
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
            var assignment = op.Get<Assignment>();

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
        ///     This code has to run for the following sample:
        ///     a = 3
        ///     b = a
        ///     a = 5 //here a is not safe to be used for future usages of b
        ///     c = b
        /// </summary>
        /// <param name="usageVariable"></param>
        /// <param name="constValues"></param>
        /// <param name="mappedValues"></param>
        private static void RemoveDefinitionsIfTheUsageIsInvalidated(
            LocalVariable usageVariable,
            Dictionary<LocalVariable, ConstValue> constValues,
            Dictionary<LocalVariable, LocalVariable> mappedValues)
        {
            if (usageVariable == null)
                return;
            var toRemove = new HashSet<LocalVariable>();
            foreach (var identifierValue in mappedValues)
            {
                if (!identifierValue.Value.Equals(usageVariable)) continue;
                toRemove.Add(identifierValue.Key);
            }
            if (toRemove.Count == 0)
                return;
            foreach (var variable in toRemove)
            {
                mappedValues.Remove(variable);
                constValues.Remove(variable);
            }
        }

        private static bool UpdateKnownUsages(LocalOperation op, Dictionary<LocalVariable, ConstValue> constValues,
            Dictionary<LocalVariable, LocalVariable> mappedValues, UseDefDescription useDef, int i)
        {
            if (mappedValues.Count == 0 && constValues.Count == 0)
                return false;
            var usagesOp = useDef.GetUsages(i);
            if (usagesOp.Length == 0)
                return false;
            var result = false;
            foreach (var usage in usagesOp)
            {
                LocalVariable mappedVar;
                if (mappedValues.TryGetValue(usage, out mappedVar))
                {
                    op.SwitchUsageWithDefinition(usage, mappedVar);
                    result = true;
                }
                ConstValue constValue;
                if (constValues.TryGetValue(usage, out constValue))
                {
                    op.SwitchUsageWithDefinition(usage, constValue);
                    result = true;
                }
            }
            return result;
        }
    }
}