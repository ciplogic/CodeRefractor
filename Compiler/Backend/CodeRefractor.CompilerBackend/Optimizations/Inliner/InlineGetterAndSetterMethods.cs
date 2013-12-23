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
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            for (int index = 0; index < intermediateCode.LocalOperations.Count; index++)
            {
                var localOperation = intermediateCode.LocalOperations[index];
                
                if (localOperation.Kind != OperationKind.Call) continue;

                var methodData = (MethodData) localOperation.Value;
                var interpreter = methodData.GetInterpreter();
                if (interpreter == null)
                    continue;

                var methodInterpreter = methodData.Info.GetInterpreter().MidRepresentation;
                if (AnalyzeFunctionIsGetter.ReadProperty(methodInterpreter)
                    || AnalyzeFunctionIsSetter.ReadProperty(methodInterpreter)
                    || AnalyzeFunctionIsEmpty.ReadProperty(methodInterpreter)
                    )
                {
                    SmallFunctionsInliner.InlineMethod(intermediateCode, interpreter, methodData, index);
                    Result = true;
                    return;
                }
            }
        }
    }
}