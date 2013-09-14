using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeFunctionIsEmpty : ResultingGlobalOptimizationPass
    {
        public const string SearchForString = "IsEmpty";

        public static bool ReadProperty(MetaMidRepresentation intermediateCode)
        {
            return intermediateCode.ReadAdditionalBool(SearchForString);
        }
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (ReadProperty(intermediateCode))
                return;
            var functionIsPure = ComputeProperty(intermediateCode);
            if (!functionIsPure) return;
            intermediateCode.SetAdditionalValue(SearchForString, true);
            Result = true;
        }

        public static bool ComputeProperty(MetaMidRepresentation intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.LocalOperations;
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