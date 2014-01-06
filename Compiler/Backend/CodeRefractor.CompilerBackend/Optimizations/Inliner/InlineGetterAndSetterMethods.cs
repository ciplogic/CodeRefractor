using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Inliner
{
    public class InlineGetterAndSetterMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            for (int index = 0; index < methodInterpreter.MidRepresentation.LocalOperations.Count; index++)
            {
                var localOperation = methodInterpreter.MidRepresentation.LocalOperations[index];
                
                if (localOperation.Kind != OperationKind.Call) continue;

                var methodData = (MethodData) localOperation.Value;
                var interpreter = methodData.GetInterpreter();
                if (interpreter == null)
                    continue;

                if (AnalyzeFunctionIsGetter.ReadProperty(interpreter)
                    || AnalyzeFunctionIsSetter.ReadProperty(interpreter)
                    || AnalyzeFunctionIsEmpty.ReadProperty(interpreter)
                    )
                {
                    SmallFunctionsInliner.InlineMethod(methodInterpreter.MidRepresentation, interpreter, methodData, index);
                    Result = true;
                    return;
                }
            }
        }
    }
}