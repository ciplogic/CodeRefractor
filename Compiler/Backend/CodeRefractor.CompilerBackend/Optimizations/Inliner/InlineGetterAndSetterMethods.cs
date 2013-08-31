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
                
                if (localOperation.Kind != LocalOperation.Kinds.Call) continue;

                var methodData = (MethodData) localOperation.Value;
                var methodBase = methodData.Info;
                var typeData = (ClassTypeData) ProgramData.UpdateType(methodBase.DeclaringType);
                var interpreter = typeData.GetInterpreter(methodBase.ToString());
                if (interpreter == null)
                    continue;

                var methodInterpreter = LinkerInterpretersTable.GetMethod(methodData.Info);
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