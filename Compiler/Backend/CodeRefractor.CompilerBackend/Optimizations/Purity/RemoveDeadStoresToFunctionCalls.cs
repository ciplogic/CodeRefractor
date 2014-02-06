using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class RemoveDeadStoresToFunctionCalls : ResultingInFunctionOptimizationPass
    {

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.LocalOperations;

            var result = false;
            var calls = new List<int>();
            for (var index = 0; index < localOperations.Count; index++)
            {
                var operation = localOperations[index];
                if (operation.Kind != OperationKind.Call)
                    continue;
                calls.Add(index);
            }
            if (calls.Count == 0) return;
            var candidates = new Dictionary<LocalVariable, int>();
            foreach (var call in calls)
            {
                var methodData = PrecomputeRepeatedUtils.GetMethodData(localOperations, call);
                if (methodData.Result == null)
                    continue;
                candidates[methodData.Result] = call;
            }

            if(candidates.Count==0)
                return;
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            for (var index = 0; index < localOperations.Count; index++)
            {
                var usages = useDef.GetUsages(index);
                foreach (var usage in usages)
                {
                    candidates.Remove(usage);
                }
                if(candidates.Count==0)
                    return;
            }

            if (candidates.Count == 0)
                return;
            Result = true;
            var toRemoveReturn = candidates.Values.ToArray();
            foreach (var index in toRemoveReturn)
            {
                var methodData = PrecomputeRepeatedUtils.GetMethodData(localOperations, index);
                methodData.Result = null;
            }

        }
    }
}