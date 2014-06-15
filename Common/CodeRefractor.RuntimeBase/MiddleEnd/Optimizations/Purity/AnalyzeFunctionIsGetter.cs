#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Purity
{
	[Optimization(Category = OptimizationCategories.Analysis)]
    public class AnalyzeFunctionIsGetter : ResultingGlobalOptimizationPass
    {
        public static bool ReadProperty(MethodInterpreter intermediateCode)
        {
            return intermediateCode.MidRepresentation.GetProperties().IsGetter;
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            if (ReadProperty(interpreter))
                return;
            var functionIsPure = ComputeFunctionIsGetter(interpreter);

            if (!functionIsPure) return;
            interpreter.MidRepresentation.GetProperties().IsGetter = true;
        }

        public static bool ComputeFunctionIsGetter(MethodInterpreter intermediateCode)
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