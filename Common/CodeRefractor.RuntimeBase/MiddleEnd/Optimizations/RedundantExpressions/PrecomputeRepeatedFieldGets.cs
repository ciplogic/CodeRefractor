#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.RedundantExpressions
{
	[Optimization(Category = OptimizationCategories.CommonSubexpressions)]
	internal class PrecomputeRepeatedFieldGets : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange,
            LocalOperation[] operations)
        {
            var localOperations = midRepresentation.MidRepresentation.UseDef.GetLocalOperations();
            var getFieldOperations = FindGetFieldOperations(midRepresentation.MidRepresentation.UseDef, startRange, endRange);
            if (getFieldOperations.Length < 2)
                return false;
            for (var i = 0; i < getFieldOperations.Length - 1; i++)
            {
                var firstOperator = localOperations.GetFieldOperation(getFieldOperations, i);
                for (var j = i + 1; j < getFieldOperations.Length; j++)
                {
                    var secondOperator = localOperations.GetFieldOperation(getFieldOperations, j);
                    if (AreDifferentOperators(firstOperator, secondOperator, getFieldOperations, i, j, localOperations))
                        continue;
                    ApplyOptimization(midRepresentation, getFieldOperations[i], getFieldOperations[j]);
                    return true;
                }
            }

            return false;
        }

        private static void ApplyOptimization(MethodInterpreter midRepresentation, int i, int j)
        {
            var localOps = midRepresentation.MidRepresentation.LocalOperations;
            var opArr = midRepresentation.MidRepresentation.UseDef.GetLocalOperations();
            var firstOperator = opArr.GetFieldOperation(i);
            var secondOperator = opArr.GetFieldOperation(j);
            var newVreg =
                midRepresentation.CreateCacheVariable(firstOperator.AssignedTo.ComputedType());
            var assignLocalOperation = PrecomputeRepeatedUtils.CreateAssignLocalOperation(firstOperator.AssignedTo,
                newVreg);
            localOps.Insert(i + 1, assignLocalOperation);

            firstOperator.AssignedTo = newVreg;

            var destAssignment = PrecomputeRepeatedUtils.CreateAssignLocalOperation(secondOperator.AssignedTo, newVreg);
            localOps.RemoveAt(j + 1);
            localOps.Insert(j + 1, destAssignment);
        }

        private static int[] FindGetFieldOperations(UseDefDescription useDef, int startRange, int endRange)
        {
            var getFieldIndexes = useDef.GetOperationsOfKind(OperationKind.GetField);
            var resultList = getFieldIndexes.Where(index => index >= startRange && index <= endRange).ToArray();
            return resultList;
        }


        private static bool AreDifferentOperators(FieldGetter firstOperator, FieldGetter secondOperator, int[] calls,
            int i,
            int j, LocalOperation[] localOperations)
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