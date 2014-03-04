using System;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering
{
    internal class ReplaceCallsToFunctionsWithUnusedArguments : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var calls = useDef.GetOperationsOfKind(OperationKind.Call).ToList();
            calls.AddRange(useDef.GetOperationsOfKind(OperationKind.CallInterface));
            calls.AddRange(useDef.GetOperationsOfKind(OperationKind.CallVirtual));
            var localOperations = useDef.GetLocalOperations();
            foreach (var call in calls)
            {
                var opCall = localOperations[call];
                var methodData = (MethodData)opCall.Value;
                var properties = methodData.Interpreter.AnalyzeProperties;
                var argumentUsages = properties.GetUsedArguments(methodData.Interpreter.MidRepresentation.Vars.Arguments);
                if(!argumentUsages.Any(it=>!it))
                    continue;
                for (var index = 0; index < argumentUsages.Length; index++)
                {
                    var argumentUsage = argumentUsages[index];
                    if(argumentUsage)continue;
                    var constValue = new ConstValue(null);
                    var paramValue = methodData.Parameters[index] as ConstValue;
                        
                    if (paramValue!=null)
                    {
                        if(paramValue.Value==null)
                            continue;
                    }
                    methodData.Parameters[index] = constValue;
                    Result = true;
                }

            }
        }
    }
}