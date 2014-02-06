using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.CompilerBackend.Optimizations.Purity;

namespace CodeRefractor.CompilerBackend.Optimizations.Inliner
{
    public class RemoveCallsToEmptyMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var toRemove = new List<int>();
            var localOperations = methodInterpreter.MidRepresentation.LocalOperations;
            for (int index = 0; index < localOperations.Count; index++)
            {
                var localOperation = localOperations[index];

                if (localOperation.Kind != OperationKind.Call) continue;
                
                var methodData = (MethodData) localOperation.Value;
                var interpreter = methodData.Info.GetInterpreter();
                if(interpreter==null)
                    continue;
                var isEmpty = interpreter.MidRepresentation.GetProperties().IsEmpty;
                if(!isEmpty)
                    continue;
                toRemove.Add(index);
            }
            if(toRemove.Count==0)
                return;
            methodInterpreter.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}