using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeFunctionIsSetter : ResultingGlobalOptimizationPass
    {
        public const string SearchForString = "IsSetter";

        public static bool ReadProperty(MetaMidRepresentation intermediateCode)
        {
            return intermediateCode.ReadAdditionalBool(SearchForString);
        }
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (ReadProperty(intermediateCode))
                return;
            var functionIsPure = ComputeFunctionPurity(intermediateCode);
            if (!functionIsPure) return;
            intermediateCode.SetAdditionalValue(SearchForString, true);
            Result = true;
        }

        public static bool ComputeFunctionPurity(MetaMidRepresentation intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.LocalOperations;
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
                    case LocalOperation.Kinds.SetField:
                    case LocalOperation.Kinds.Assignment:
                    case LocalOperation.Kinds.AlwaysBranch:
                    case LocalOperation.Kinds.Label:
                    case LocalOperation.Kinds.Return:
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }
    }
}