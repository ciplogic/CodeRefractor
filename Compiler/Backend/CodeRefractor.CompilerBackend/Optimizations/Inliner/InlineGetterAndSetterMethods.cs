using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using Compiler.CodeWriter.Linker;

namespace CodeRefractor.CompilerBackend.Optimizations.Inliner
{
    public class InlineGetterAndSetterMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.UseDef.GetLocalOperations();
            for (int index = 0; index < localOperations.Length; index++)
            {
                var localOperation = localOperations[index];
                
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