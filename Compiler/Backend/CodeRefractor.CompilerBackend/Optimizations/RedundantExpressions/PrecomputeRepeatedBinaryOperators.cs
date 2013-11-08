﻿using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions
{
    class PrecomputeRepeatedBinaryOperators : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MetaMidRepresentation midRepresentation, int startRange, int endRange)
        {
            var localOperations = midRepresentation.LocalOperations;
            var calls = FindBinaryOperators(localOperations, startRange, endRange);
            if (calls.Count < 2)
                return false;
            for (var i = 0; i < calls.Count - 1; i++)
            {
                var firstOperator = localOperations.GetBinaryOperator(calls, i);
                for (var j = i + 1; j < calls.Count; j++)
                {
                    var secondOperator = localOperations.GetBinaryOperator(calls, j);
                    if (AreDifferentOperators(firstOperator, secondOperator, calls, i, j, localOperations)) continue;
                    ApplyOptimization(midRepresentation, calls[i], calls[j]);
                    return true;
                }
            }

            return false;
        }
        private static void ApplyOptimization(MetaMidRepresentation midRepresentation, int i, int j)
        {
            var localOps = midRepresentation.LocalOperations;
            var firstOperator = localOps.GetBinaryOperator(i);
            var secondOperator = localOps.GetBinaryOperator(j);
            var newVreg = midRepresentation.CreateCacheVariable(firstOperator.ComputedType());
            var assignLocalOperation = PrecomputeRepeatedUtils.CreateAssignLocalOperation(firstOperator.AssignedTo, newVreg);
            localOps.Insert(i + 1, assignLocalOperation);
            
            firstOperator.AssignedTo = newVreg;

            var destAssignment = PrecomputeRepeatedUtils.CreateAssignLocalOperation(secondOperator.AssignedTo, newVreg);
            localOps.RemoveAt(j + 1);
            localOps.Insert(j + 1, destAssignment);
        }

        private static List<int> FindBinaryOperators(List<LocalOperation> localOperations, int startRange, int endRange)
        {

            var calls = new List<int>();
            for (var index = startRange; index <= endRange; index++)
            {
                var operation = localOperations[index];
                if (operation.Kind != OperationKind.BinaryOperator)
                    continue;
                calls.Add(index);
            }
            return calls;
        }


        private static bool AreDifferentOperators(BinaryOperator firstOperator, BinaryOperator secondOperator, List<int> calls, int i,
                                                 int j, List<LocalOperation> localOperations)
        {
            if (firstOperator.Name != secondOperator.Name)
                return true;
            if (!firstOperator.Left.Equals(secondOperator.Left))
                return true;
            if (!firstOperator.Right.Equals(secondOperator.Right))
                return true;
            var definitions = new HashSet<LocalVariable>();
            if (firstOperator.Left is LocalVariable)
                definitions.Add((LocalVariable)firstOperator.Left);
            if (firstOperator.Right is LocalVariable)
                definitions.Add((LocalVariable)firstOperator.Right);
            var isReassigned = false;
            for (var index = calls[i] + 1; index < calls[j]; index++)
            {
                var op = localOperations[index];
                var def = op.GetUseDefinition();
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