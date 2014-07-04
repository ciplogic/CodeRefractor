#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Purity
{
	[Optimization(Category = OptimizationCategories.UseDef)]
	public class RemoveDeadStoresToFunctionCalls : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
			var useDef = interpreter.MidRepresentation.UseDef;
			var localOperations = useDef.GetLocalOperations();
			var calls = useDef.GetOperationsOfKind(OperationKind.Call);
            if (calls.Length == 0) return;
            var candidates = new Dictionary<LocalVariable, int>();
            foreach (var call in calls)
            {
                var methodData = (CallMethodStatic) localOperations[call];
                if (methodData.Result == null)
                    continue;
                candidates[methodData.Result] = call;
            }

            if (candidates.Count == 0)
                return;
            for (var index = 0; index < localOperations.Length; index++)
            {
                var usages = useDef.GetUsages(index);
                foreach (var usage in usages)
                {
                    candidates.Remove(usage);
                }
                if (candidates.Count == 0)
                    return;
            }

            if (candidates.Count == 0)
                return;
            Result = true;
            var toRemoveReturn = candidates.Values.ToArray();
            foreach (var index in toRemoveReturn)
            {
                var methodData = (CallMethodStatic) localOperations[index];
                methodData.Result = null;
            }
        }
    }
}