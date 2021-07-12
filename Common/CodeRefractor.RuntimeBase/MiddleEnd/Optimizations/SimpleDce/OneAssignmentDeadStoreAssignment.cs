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
class OneAssignmentDeadStoreAssignment : OptimizationPassBase
    {
        public OneAssignmentDeadStoreAssignment()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var localOperations = useDef.GetLocalOperations();
            var constValues = GetAssignToConstOperations(localOperations, useDef);

            if (constValues.Count == 0)
                return false;

            localOperations = useDef.GetLocalOperations();
            var result = false;
            for (var index = 0; index < localOperations.Length; index++)
            {
                var op = localOperations[index];
                var variableUsages = useDef.GetUsages(index);
                if (variableUsages.Length == 0)
                    continue;
                foreach (var variable in variableUsages)
                {
                    ConstValue constMappedValue;
                    if (constValues.TryGetValue(variable, out constMappedValue))
                    {
                        op.SwitchUsageWithDefinition(variable, constMappedValue);
                        result = true;
                    }
                }
            }
            return result;
        }

        Dictionary<LocalVariable, ConstValue> GetAssignToConstOperations(LocalOperation[] localOperations,
            UseDefDescription useDef)
        {
            var constValues = new Dictionary<LocalVariable, ConstValue>();
            var assignmentIds = useDef.GetOperationsOfKind(OperationKind.Assignment);
            foreach (var index in assignmentIds)
            {
                var op = localOperations[index];
                var assign = (Assignment)op;
                var assignedTo = assign.AssignedTo;
                if (assignedTo.Kind == VariableKind.Argument)
                    continue;
                var constAssignedValue = assign.Right as ConstValue;
                if (constAssignedValue == null)
                    continue;
                constValues[assignedTo] = constAssignedValue;
            }
            for (var index = 0; index < localOperations.Length; index++)
            {
                var op = localOperations[index];
                var definition = useDef.GetDefinition(index);
                if (definition == null)
                    continue;
                if (op.Kind != OperationKind.Assignment)
                {
                    constValues.Remove(definition);
                    continue;
                }
                var assign = (Assignment)op;
                var constAssignedValue = assign.Right as ConstValue;
                if (constAssignedValue == null)
                {
                    constValues.Remove(definition);
                    continue;
                }
                ConstValue constDefValue;
                if (!constValues.TryGetValue(definition, out constDefValue)) continue;
                if (constDefValue.Value != constAssignedValue.Value)
                    constValues.Remove(definition);
            }
            return constValues;
        }
    }
}