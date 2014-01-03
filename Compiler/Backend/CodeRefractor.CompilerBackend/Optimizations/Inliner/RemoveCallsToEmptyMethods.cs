using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.CompilerBackend.Optimizations.Purity;

namespace CodeRefractor.CompilerBackend.Optimizations.Inliner
{
    public class RemoveCallsToEmptyMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var toRemove = new List<int>();
            for (int index = 0; index < intermediateCode.LocalOperations.Count; index++)
            {
                var localOperation = intermediateCode.LocalOperations[index];

                if (localOperation.Kind != OperationKind.Call) continue;
                
                var methodData = (MethodData) localOperation.Value;
                var isEmpty = methodData.Info.GetInterpreter().MidRepresentation.GetProperties().IsEmpty;
                if(!isEmpty)
                    continue;
                toRemove.Add(index);
            }
            if(toRemove.Count==0)
                return;
            intermediateCode.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}