using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions
{
    internal class PrecomputeRepeatedFieldGets : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange)
        {
            var localOperations = midRepresentation.MidRepresentation.LocalOperations;
            var getFieldOperations = FindGetFieldOperations(localOperations, startRange, endRange);
            if (getFieldOperations.Count < 2)
                return false;
            for (var i = 0; i < getFieldOperations.Count - 1; i++)
            {
                var firstOperator = localOperations.GetFieldOperation(getFieldOperations, i);
                for (var j = i + 1; j < getFieldOperations.Count; j++)
                {
                    var secondOperator = localOperations.GetFieldOperation(getFieldOperations, j);
                    if (AreDifferentOperators(firstOperator, secondOperator, getFieldOperations, i, j, localOperations)) continue;
                    ApplyOptimization(midRepresentation, getFieldOperations[i], getFieldOperations[j]);
                    return true;
                }
            }

            return false;
        }
        private static void ApplyOptimization(MethodInterpreter midRepresentation, int i, int j)
        {
            var localOps = midRepresentation.MidRepresentation.LocalOperations;
            var firstOperator = localOps.GetFieldOperation(i);
            var secondOperator = localOps.GetFieldOperation(j);
            var newVreg = midRepresentation.MidRepresentation.CreateCacheVariable(firstOperator.AssignedTo.ComputedType());
            var assignLocalOperation = PrecomputeRepeatedUtils.CreateAssignLocalOperation(firstOperator.AssignedTo, newVreg);
            localOps.Insert(i + 1, assignLocalOperation);

            firstOperator.AssignedTo = newVreg;

            var destAssignment = PrecomputeRepeatedUtils.CreateAssignLocalOperation(secondOperator.AssignedTo, newVreg);
            localOps.RemoveAt(j + 1);
            localOps.Insert(j + 1, destAssignment);
        }

        private static List<int> FindGetFieldOperations(List<LocalOperation> localOperations, int startRange, int endRange)
        {

            var calls = new List<int>();
            for (var index = startRange; index <= endRange; index++)
            {
                var operation = localOperations[index];
                if (operation.Kind != OperationKind.GetField)
                    continue;
                calls.Add(index);
            }
            return calls;
        }


        private static bool AreDifferentOperators(FieldGetter firstOperator, FieldGetter secondOperator, List<int> calls, int i,
                                                  int j, List<LocalOperation> localOperations)
        {
            if (firstOperator.FieldName != secondOperator.FieldName)
                return true;
            if (!firstOperator.Instance.Equals(secondOperator.Instance))
                return true;
            var definitions = new HashSet<LocalVariable>
                {
                    firstOperator.Instance
                };
            var isReassigned = false;
            for (var index = calls[i] + 1; index < calls[j]; index++)
            {
                var op = localOperations[index];
                var def = op.GetDefinition();
                if (def == null)
                    continue;
                if (!definitions.Contains(def)) continue;
                isReassigned = true;
                break;
            }
            return isReassigned;
        }
    }
}