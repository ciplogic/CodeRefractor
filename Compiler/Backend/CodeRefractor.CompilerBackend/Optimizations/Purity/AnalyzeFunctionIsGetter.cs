using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeFunctionIsGetter : ResultingGlobalOptimizationPass
    {

        public static bool ReadProperty(MethodInterpreter intermediateCode)
        {
            return intermediateCode.MidRepresentation.GetProperties().IsGetter;
        }
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            if (ReadProperty(methodInterpreter))
                return;
            var functionIsPure = ComputeFunctionPurity(methodInterpreter);
            
            if (!functionIsPure) return;
            methodInterpreter.MidRepresentation.GetProperties().IsGetter = true; 
        }

        public static bool ComputeFunctionPurity(MethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.MidRepresentation.LocalOperations;
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