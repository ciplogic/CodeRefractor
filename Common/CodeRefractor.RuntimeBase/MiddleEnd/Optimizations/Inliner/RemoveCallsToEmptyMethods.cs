#region Uses

using System.Collections.Generic;
using CodeRefractor.Analyze;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Inliner
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class RemoveCallsToEmptyMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var toRemove = new List<int>();
            var useDef = interpreter.MidRepresentation.UseDef;
            var localOperations = useDef.GetLocalOperations();
            var calls = useDef.GetOperationsOfKind(OperationKind.Call);
            foreach (var index in calls)
            {
                var localOperation = localOperations[index];

                if (localOperation.Kind != OperationKind.Call) continue;

                var methodData = (CallMethodStatic) localOperation;
                var callInterpreter = methodData.Info.GetInterpreter(Closure);
                if (callInterpreter == null)
                    continue;
                var isEmpty = callInterpreter.AnalyzeProperties.IsEmpty;
                if (!isEmpty)
                    continue;
                toRemove.Add(index);
            }
            if (toRemove.Count == 0)
                return;
            interpreter.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}