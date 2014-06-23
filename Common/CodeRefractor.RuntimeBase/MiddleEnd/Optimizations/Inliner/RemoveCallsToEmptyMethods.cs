#region Usings

using System.Collections.Generic;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Inliner
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class RemoveCallsToEmptyMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var toRemove = new List<int>();
            var localOperations = methodInterpreter.MidRepresentation.UseDef.GetLocalOperations();
            for (var index = 0; index < localOperations.Length; index++)
            {
                var localOperation = localOperations[index];

                if (localOperation.Kind != OperationKind.Call) continue;

                var methodData = (MethodData) localOperation;
                var interpreter = methodData.Info.GetInterpreter(Runtime);
                if (interpreter == null)
                    continue;
                var isEmpty = interpreter.MidRepresentation.GetProperties().IsEmpty;
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