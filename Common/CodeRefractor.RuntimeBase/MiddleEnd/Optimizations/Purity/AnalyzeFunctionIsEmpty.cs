#region Uses

using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Purity
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    public class AnalyzeFunctionIsEmpty : ResultingGlobalOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            if (ReadProperty(interpreter))
                return;
            var isEmtpy = ComputeProperty(interpreter);
            var previous = interpreter.AnalyzeProperties.IsEmpty;
            interpreter.AnalyzeProperties.IsEmpty = isEmtpy;
            if (previous != isEmtpy)
                Result = true;
        }

        public static bool ReadProperty(CilMethodInterpreter intermediateCode)
        {
            return intermediateCode.AnalyzeProperties.IsEmpty;
        }

        static bool ComputeProperty(CilMethodInterpreter intermediateCode)
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