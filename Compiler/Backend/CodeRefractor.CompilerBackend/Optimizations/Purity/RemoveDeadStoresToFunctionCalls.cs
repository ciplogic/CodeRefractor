using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class RemoveDeadStoresToFunctionCalls : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();

            var calls = methodInterpreter.MidRepresentation.UseDef.GetOperations(OperationKind.Call);
            if (calls.Length == 0) return;
            var candidates = new Dictionary<LocalVariable, int>();
            foreach (var call in calls)
            {
                var methodData = (MethodData)localOperations[call].Value;
                if (methodData.Result == null)
                    continue;
                candidates[methodData.Result] = call;
            }

            if(candidates.Count==0)
                return;
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            for (var index = 0; index < localOperations.Length; index++)
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
                var methodData = (MethodData)localOperations[index].Value;
                methodData.Result = null;
            }

        }
    }
}