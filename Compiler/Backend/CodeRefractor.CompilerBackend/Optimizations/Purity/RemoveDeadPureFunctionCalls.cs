using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class RemoveDeadPureFunctionCalls : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange, LocalOperation[] operations)
        {
            var localOperations = midRepresentation.MidRepresentation.LocalOperations;

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
            midRepresentation.MidRepresentation.DeleteInstructions(toRemove);
            toRemove.Clear();
            return true;
        }
    }
}