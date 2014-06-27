#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.Purity
{
	[Optimization(Category = OptimizationCategories.Analysis)]
    public class AnalyzeFunctionIsEmpty : ResultingGlobalOptimizationPass
    {
        public static bool ReadProperty(MethodInterpreter intermediateCode)
        {
            return intermediateCode.MidRepresentation.GetProperties().IsEmpty;
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            if (ReadProperty(interpreter))
                return;
            var isEmtpy = ComputeProperty(interpreter);
            var previous = interpreter.MidRepresentation.GetProperties().IsEmpty;
            interpreter.MidRepresentation.GetProperties().IsEmpty = isEmtpy;
            if (previous != isEmtpy)
                Result = true;
        }

        private static bool ComputeProperty(MethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.MidRepresentation.UseDef.GetLocalOperations();
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
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