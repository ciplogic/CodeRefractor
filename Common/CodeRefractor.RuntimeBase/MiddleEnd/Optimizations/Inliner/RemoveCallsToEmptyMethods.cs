#region Usings

using System.Collections.Generic;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.Optimizations.Purity;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Inliner
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class RemoveCallsToEmptyMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter methodInterpreter)
        {
            var toRemove = new List<int>();
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            var localOperations = useDef.GetLocalOperations();
            var calls = useDef.GetOperationsOfKind(OperationKind.Call);
            foreach (var index in calls)
            {
                var localOperation = localOperations[index];

                if (localOperation.Kind != OperationKind.Call) continue;

                var methodData = (CallMethodStatic) localOperation;
                var interpreter = methodData.Info.GetInterpreter(Closure);
                if (interpreter == null)
                    continue;
                var isEmpty = interpreter.AnalyzeProperties.IsEmpty;
                if (!isEmpty)
                    continue;
                toRemove.Add(index);
            }
            if (toRemove.Count == 0)
                return;
            methodInterpreter.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}