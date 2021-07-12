#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Purity
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    public class AnalyzeFunctionIsSetter : ResultingGlobalOptimizationPass
    {
        public static bool ReadProperty(CilMethodInterpreter intermediateCode)
        {
            return intermediateCode.AnalyzeProperties.IsSetter;
        }

        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            if (ReadProperty(interpreter))
                return;
            var functionIsPure = ComputeFunctionPurity(interpreter);
            if (!functionIsPure) return;
            interpreter.AnalyzeProperties.IsSetter = true;
        }

        static bool ComputeFunctionPurity(CilMethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.MidRepresentation.UseDef.GetLocalOperations();
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
                    case OperationKind.SetField:
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