using System;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class RemoveDeadStoresToFunctionCalls : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MetaMidRepresentation midRepresentation, int startRange, int endRange)
        {
            var localOperations = midRepresentation.LocalOperations;

            var result = false;
            var calls = localOperations
                .Select((item, index) => new Tuple<LocalOperation,int>(item,index))
                .Where(op => op.Item1.Kind == OperationKind.Call)
                .Select(tuple=>tuple.Item2)
                .ToArray();
            foreach (var call in calls)
            {
                var methodData = PrecomputeRepeatedPureFunctionCall.GetMethodData(localOperations, call);
                if (methodData.Result == null)
                {
                    continue;
                }
                var resultUsages = localOperations.GetVariableUsages(methodData.Result);
                if (resultUsages.Count != 0) continue;
                result = true;
                methodData.Result = null;
            }
            return result;
        }
    }
}