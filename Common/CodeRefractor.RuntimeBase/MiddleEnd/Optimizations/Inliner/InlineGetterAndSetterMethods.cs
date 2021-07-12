#region Uses

using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.Optimizations.Purity;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Inliner
{
    [Optimization(Category = OptimizationCategories.Inliner)]
    public class InlineGetterAndSetterMethods : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            for (var index = 0; index < localOperations.Length; index++)
            {
                var localOperation = localOperations[index];

                if (localOperation.Kind != OperationKind.Call) continue;

                var methodData = (CallMethodStatic) localOperation;
                var cilMethodInterpreter = methodData.GetInterpreter(Closure) as CilMethodInterpreter;
                if (cilMethodInterpreter == null)
                    continue;

                if (AnalyzeFunctionIsGetter.ReadProperty(cilMethodInterpreter)
                    || AnalyzeFunctionIsSetter.ReadProperty(cilMethodInterpreter)
                    || AnalyzeFunctionIsEmpty.ReadProperty(cilMethodInterpreter)
                    )
                {
                    SmallFunctionsInliner.InlineMethod(interpreter.MidRepresentation, methodData,
                        index);
                    Result = true;
                    return;
                }
            }
        }
    }
}