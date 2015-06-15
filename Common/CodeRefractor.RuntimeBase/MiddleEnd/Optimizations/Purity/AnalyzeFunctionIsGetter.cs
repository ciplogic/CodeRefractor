#region Uses

using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Purity
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    public class AnalyzeFunctionIsGetter : ResultingGlobalOptimizationPass
    {
        public static bool ReadProperty(CilMethodInterpreter intermediateCode)
        {
            return intermediateCode.AnalyzeProperties.IsGetter;
        }

        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            if (ReadProperty(interpreter))
                return;
            var functionIsPure = ComputeFunctionIsGetter(interpreter);

            if (!functionIsPure) return;
            interpreter.AnalyzeProperties.IsGetter = true;
        }

        public static bool ComputeFunctionIsGetter(CilMethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.MidRepresentation.UseDef.GetLocalOperations();
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
                    case OperationKind.GetField:
                    case OperationKind.Assignment:
                    case OperationKind.AlwaysBranch:
                    case OperationKind.Label:
                    case OperationKind.Return:
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
    }
}