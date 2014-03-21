#region Usings

using System.Collections.Generic;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class RemoveDeadPureFunctionCalls : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            var localOperations = useDef.GetLocalOperations();

            var toRemove = new List<int>();
            var callIndices = useDef.GetOperationsOfKind(OperationKind.Call);
            foreach (var index in callIndices)
            {
                var operation = localOperations[index];
                var methodData = operation.Get<MethodData>();
                var interpreter = methodData.GetInterpreter(Runtime);
                if (interpreter == null)
                    continue;
                if(methodData.Result!=null)
                    continue;
                var properties = interpreter.AnalyzeProperties;
                if (properties.IsReadOnly || properties.IsPure)
                {
                    toRemove.Add(index);
                }
            }
            if (toRemove.Count == 0)
                return;
            methodInterpreter.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}