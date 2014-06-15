#region Usings

using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.Purity;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Inliner
{
    public class InlineGetterAndSetterMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.UseDef.GetLocalOperations();
            for (var index = 0; index < localOperations.Length; index++)
            {
                var localOperation = localOperations[index];

                if (localOperation.Kind != OperationKind.Call) continue;

                var methodData = (MethodData) localOperation.Value;
                var interpreter = methodData.GetInterpreter(Runtime);
                if (interpreter == null)
                    continue;

                if (AnalyzeFunctionIsGetter.ReadProperty(interpreter)
                    || AnalyzeFunctionIsSetter.ReadProperty(interpreter)
                    || AnalyzeFunctionIsEmpty.ReadProperty(interpreter)
                    )
                {
                    SmallFunctionsInliner.InlineMethod(methodInterpreter.MidRepresentation, methodData,
                        index);
                    Result = true;
                    return;
                }
            }
        }
    }
}