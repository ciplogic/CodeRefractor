#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.RedundantExpressions
{
	[Optimization(Category = OptimizationCategories.CommonSubexpressionsElimination)]
	internal class PrecomputeRepeatedUnaryOperators : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange,
            LocalOperation[] operations)
        {
            var localOperations = midRepresentation.MidRepresentation.LocalOperations;
            var calls = FindUnaryOperators(localOperations, startRange, endRange);
            if (calls.Count < 2)
                return false;
            for (var i = 0; i < calls.Count - 1; i++)
            {
                var firstOperator = localOperations.GetUnaryOperator(calls, i);
                for (var j = i + 1; j < calls.Count; j++)
                {
                    var secondOperator = localOperations.GetUnaryOperator(calls, j);
                    if (AreDifferentOperators(firstOperator, secondOperator, calls, i, j, localOperations)) continue;
                    ApplyOptimization(midRepresentation, calls[i], calls[j]);
                    return true;
                }
            }

            return false;
        }

        private static void ApplyOptimization(MethodInterpreter midRepresentation, int i, int j)
        {
            var localOps = midRepresentation.MidRepresentation.LocalOperations;
            var firstOperator = localOps.GetUnaryOperator(i);
            var secondOperator = localOps.GetUnaryOperator(j);
            var newVreg =
                midRepresentation.CreateCacheVariable(firstOperator.AssignedTo.ComputedType());
            var assignLocalOperation = PrecomputeRepeatedUtils.CreateAssignLocalOperation(firstOperator.AssignedTo,
                newVreg);

            firstOperator.AssignedTo = newVreg;
            localOps.Insert(i + 1, assignLocalOperation);

            var destAssignment = PrecomputeRepeatedUtils.CreateAssignLocalOperation(secondOperator.AssignedTo, newVreg);
            localOps.RemoveAt(j + 1);
            localOps.Insert(j + 1, destAssignment);
        }

        private static List<int> FindUnaryOperators(List<LocalOperation> localOperations, int startRange, int endRange)
        {
            var calls = new List<int>();
            for (var index = startRange; index <= endRange; index++)
            {
                var operation = localOperations[index];
                if (operation.Kind != OperationKind.UnaryOperator)
                    continue;
                calls.Add(index);
            }
            return calls;
        }


        private static bool AreDifferentOperators(UnaryOperator firstOperator, UnaryOperator secondOperator,
            IList<int> calls, int i,
            int j, List<LocalOperation> localOperations)
        {
            if (firstOperator.Name != secondOperator.Name)
                return true;
            if (!firstOperator.Left.Equals(secondOperator.Left))
                return true;
            if (!(firstOperator.Left is LocalVariable))
                return true;
            var definitions = (LocalVariable) firstOperator.Left;
            var isReassigned = false;
            for (var index = calls[i] + 1; index < calls[j]; index++)
            {
                var op = localOperations[index];
                var def = op.GetDefinition();
                if (def == null)
                    continue;
                if (!definitions.Equals(def)) continue;
                isReassigned = true;
                break;
            }
            return isReassigned;
        }
    }
}