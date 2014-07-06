#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.RedundantExpressions
{
	[Optimization(Category = OptimizationCategories.CommonSubexpressionsElimination)]
	internal class PrecomputeRepeatedFieldGets : BlockOptimizationPass
    {
        public override bool OptimizeBlock(CilMethodInterpreter midRepresentation, int startRange, int endRange,
            LocalOperation[] operations)
        {
            var localOperations = midRepresentation.MidRepresentation.UseDef.GetLocalOperations();
            var getFieldOperations = FindGetFieldOperations(midRepresentation.MidRepresentation.UseDef, startRange, endRange);
            if (getFieldOperations.Length < 2)
                return false;
            return ProcessOptimizeBlock(midRepresentation, getFieldOperations, localOperations);
        }

        private static bool ProcessOptimizeBlock(CilMethodInterpreter midRepresentation, int[] getFieldOperations, LocalOperation[] localOperations)
	    {
            var instructionRange = midRepresentation.MidRepresentation.LocalOperations;
	        for (var i = 0; i < getFieldOperations.Length - 1; i++)
	        {
	            var firstOperator = localOperations.GetFieldOperation(getFieldOperations, i);
	            for (var j = i + 1; j < getFieldOperations.Length; j++)
	            {
	                var secondOperator = localOperations.GetFieldOperation(getFieldOperations, j);
	                if (AreDifferentOperators(firstOperator, secondOperator, getFieldOperations, i, j, localOperations))
	                    continue;
	                var firstGet = getFieldOperations[i];
	                var secondGet = getFieldOperations[j];
	                ApplyOptimization(midRepresentation, firstGet, secondGet);
	                return true;
	            }
	        }

	        return false;
	    }

        private static void ApplyOptimization(CilMethodInterpreter midRepresentation, int i, int j)
        {
            var localOps = midRepresentation.MidRepresentation.LocalOperations;
            var firstOperator = localOps[i].Get<GetField>();
            var secondOperator = localOps[j].Get<GetField>();
            var newVreg =
                midRepresentation.CreateCacheVariable(firstOperator.AssignedTo.ComputedType());
            var assignLocalOperation = PrecomputeRepeatedUtils.CreateAssignLocalOperation(firstOperator.AssignedTo,
                newVreg);
            localOps.Insert(i + 1, assignLocalOperation);

            firstOperator.AssignedTo = newVreg;

            var destAssignment = PrecomputeRepeatedUtils.CreateAssignLocalOperation(secondOperator.AssignedTo, newVreg);
            localOps[j + 1]  = destAssignment;
        }

        private static int[] FindGetFieldOperations(UseDefDescription useDef, int startRange, int endRange)
        {
            var getFieldIndexes = useDef.GetOperationsOfKind(OperationKind.GetField);
            var resultList = getFieldIndexes.Where(index => index >= startRange && index <= endRange).ToArray();
            return resultList;
        }


        private static bool AreDifferentOperators(GetField firstOperator, GetField secondOperator, int[] calls,
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