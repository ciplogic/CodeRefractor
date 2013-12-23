using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.CompilerBackend.Linker;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeFunctionPurity : ResultingGlobalOptimizationPass
    {
        public const string IsPureString = "IsPure";

        public static bool ReadPurity(MetaMidRepresentation intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var additionalData = intermediateCode.AuxiliaryObjects;

            object isPureData; 
            return additionalData.TryGetValue(IsPureString, out isPureData);
        }
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (ReadPurity(intermediateCode))
                return;
            var functionIsPure = ComputeFunctionPurity(intermediateCode);
            var additionalData = intermediateCode.AuxiliaryObjects;
            if (!functionIsPure) return;
            additionalData[IsPureString] = true;
            Result = true;
        }

        public static bool ComputeFunctionPurity(MetaMidRepresentation intermediateCode)
        {
            if(intermediateCode==null)
                return false;
            var operations = intermediateCode.LocalOperations;
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
                    case OperationKind.SetStaticField:
                    case OperationKind.GetStaticField:
                    case OperationKind.CallRuntime:
                    case OperationKind.SetField:
                        return false;
                        
                    case OperationKind.Call:
                        var operationData = (MethodData)localOperation.Value;
                        var readPurity = LinkerInterpretersTableUtils.ReadPurity(operationData.Info);
                        if (!readPurity)
                            return false;
                        break;

                    case OperationKind.BranchOperator:
                    case OperationKind.AlwaysBranch:
                    case OperationKind.UnaryOperator:
                    case OperationKind.BinaryOperator:
                    case OperationKind.Assignment:
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