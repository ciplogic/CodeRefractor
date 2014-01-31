using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    class OneAssignmentDeadStoreAssignment : ResultingInFunctionOptimizationPass
    {
        readonly Dictionary<LocalVariable,ConstValue> _constValues = new Dictionary<LocalVariable, ConstValue>();
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();
            _constValues.Clear();

            GetAssignToConstOperations(localOperations);

            if (_constValues.Count == 0)
                return;

            localOperations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            for (int index = 0; index < localOperations.Length; index++)
            {
                var op = localOperations[index];
                var variableUsages = useDef.GetUsages(index);
                if (variableUsages.Length == 0)
                    continue;
                foreach (var variable in variableUsages)
                {
                    ConstValue constMappedValue;
                    if (_constValues.TryGetValue(variable, out constMappedValue))
                    {
                        op.SwitchUsageWithDefinition(variable, constMappedValue);
                        Result = true;
                    }
                }

            }
        }

        private static HashSet<LocalVariable> ComputeDefinedOnce(Dictionary<int, LocalOperation> assignToConstOperations)
        {
            var definedOnce = new HashSet<LocalVariable>();
            var definedMany = new HashSet<LocalVariable>();
            foreach (var op in assignToConstOperations)
            {
                var left = op.Value.GetAssignment().AssignedTo;
                if (definedOnce.Contains(left))
                {
                    definedMany.Add(left);
                }
                else
                {
                    definedOnce.Add(left);
                }
            }
            foreach (var many in definedMany)
            {
                definedOnce.Remove(many);
            }
            return definedOnce;
        }

        private void GetAssignToConstOperations(LocalOperation[] localOperations)
        {
            for (int index = 0; index < localOperations.Length; index++)
            {
                var op = localOperations[index];
                var opKind = op.Kind;
                if (opKind != OperationKind.Assignment) continue;
                var assign = op.GetAssignment();
                var assignedTo = assign.AssignedTo;
                if (assignedTo.Kind == VariableKind.Argument)
                    continue;
                var constAssignedValue = assign.Right as ConstValue;
                if (constAssignedValue==null)
                    continue;
                ConstValue constVal;
                if (_constValues.TryGetValue(assignedTo, out constVal))
                {
                    if (constVal == null)
                        continue;
                    if (constVal.Value == constAssignedValue.Value)
                        continue;
                    _constValues[assignedTo] = null;
                }
                else
                {
                    _constValues[assignedTo] = constAssignedValue;
                }
            }
            var toRemove = new List<LocalVariable>();
            foreach (var constValue in _constValues)
            {
                if(constValue.Value==null)//const defined multiple times
                    toRemove.Add(constValue.Key);
            }
            foreach (var localVariable in toRemove)
            {
                _constValues.Remove(localVariable);
            }
        }
    }
}