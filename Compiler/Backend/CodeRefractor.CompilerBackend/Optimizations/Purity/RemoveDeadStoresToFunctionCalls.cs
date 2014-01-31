using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class RemoveDeadStoresToFunctionCalls : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange)
        {
            var localOperations = midRepresentation.MidRepresentation.LocalOperations;

            var result = false;
            var calls = new List<int>();
            for (var index = 0; index < localOperations.Count; index++)
            {
                var operation = localOperations[index];
                if(operation.Kind!=OperationKind.Call)
                    continue;
                calls.Add(index);
            }
            if (calls.Count == 0)
                return false;
            foreach (var call in calls)
            {
                var methodData = PrecomputeRepeatedUtils.GetMethodData(localOperations, call);
                if (methodData.Result == null)
                    continue;
                var resultUsages = localOperations.GetVariableUsages(methodData.Result);
                if (resultUsages.Count != 0) continue;
                result = true;
                methodData.Result = null;
            }
            return result;
        }
    }
}