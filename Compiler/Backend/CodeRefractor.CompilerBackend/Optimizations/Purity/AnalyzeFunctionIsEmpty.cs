using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeFunctionIsEmpty : ResultingGlobalOptimizationPass
    {
        public static bool ReadProperty(MetaMidRepresentation intermediateCode)
        {
            return intermediateCode.GetProperties().IsEmpty;
        }

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (ReadProperty(intermediateCode))
                return;
            var isEmtpy = ComputeProperty(intermediateCode);
            var previous = intermediateCode.GetProperties().IsEmpty;
            intermediateCode.GetProperties().IsEmpty = isEmtpy;
            if (previous!= isEmtpy) 
                Result = true;
        }

        private static bool ComputeProperty(MetaMidRepresentation intermediateCode)
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