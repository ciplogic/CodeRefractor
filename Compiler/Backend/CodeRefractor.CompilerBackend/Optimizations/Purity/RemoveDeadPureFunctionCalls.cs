using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class RemoveDeadPureFunctionCalls : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.LocalOperations;

            var toRemove = new List<int>();
            for (var index = 0; index < localOperations.Count; index++)
            {
                var operation = localOperations[index];
                if(operation.Kind!=OperationKind.Call)
                    continue;

                var methodData = operation.Get<MethodData>();
                var interpreter = methodData.GetInterpreter();
                if(interpreter==null)
                    continue;
                var properties = interpreter.AnalyzeProperties;
                if (properties.IsReadOnly || properties.IsPure)
                {
                    toRemove.Add(index);
                }
            }
            if(toRemove.Count==0)
                return;
            methodInterpreter.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}