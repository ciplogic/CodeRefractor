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
            if (intermediateCode == null)
                return false;
            var additionalData = intermediateCode.AuxiliaryObjects;

            object isPureData;
            return additionalData.TryGetValue(SearchForString, out isPureData);
        }
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (ReadProperty(intermediateCode))
                return;
            var functionIsPure = ComputeProperty(intermediateCode);
            var additionalData = intermediateCode.AuxiliaryObjects;
            if (!functionIsPure) return;
            additionalData[SearchForString] = true;
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