using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class RemoveDeadPureFunctionCalls : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MetaMidRepresentation midRepresentation, int startRange, int endRange)
        {
            var localOperations = midRepresentation.LocalOperations;

            var calls = PrecomputeRepeatedPureFunctionCall.FindCallsToPureFunctions(localOperations, startRange, endRange);
            var toRemove = new HashSet<int>();
            foreach (var call in calls)
            {
                var methodData = PrecomputeRepeatedUtils.GetMethodData(localOperations, call);
                if (methodData.Result == null)
                {
                    toRemove.Add(call);
                    continue;
                }
                var resultUsages = localOperations.GetVariableUsages(methodData.Result);
                if (resultUsages.Count == 0)
                    toRemove.Add(call);
            }
            if (toRemove.Count == 0)
                return false;
            midRepresentation.DeleteInstructions(toRemove);
            return true;
        }
    }
}