#region Uses

using System.Linq;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.EscapeAndLowering
{
    [Optimization(Category = OptimizationCategories.Propagation)]
    internal class ReplaceCallsToFunctionsWithUnusedArguments : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
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
                var methodData = (CallMethodStatic) opCall;
                var properties = methodData.Interpreter.AnalyzeProperties;
                var argumentUsages = properties.GetUsedArguments(methodData.Interpreter.AnalyzeProperties.Arguments);
                if (!argumentUsages.Any(it => !it))
                    continue;
                for (var index = 0; index < argumentUsages.Length; index++)
                {
                    var argumentUsage = argumentUsages[index];
                    if (argumentUsage) continue;
                    var constValue = new ConstValue(null);
                    var paramValue = methodData.Parameters[index] as ConstValue;

                    if (paramValue != null)
                    {
                        if (paramValue.Value == null)
                            continue;
                    }
                    methodData.Parameters[index] = constValue;
                    Result = true;
                }
            }
        }
    }
}