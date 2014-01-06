using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeFunctionNoStaticSideEffects : ResultingGlobalOptimizationPass
    {
        public static bool ReadPurity(MethodInterpreter intermediateCode)
        {
            return intermediateCode.MidRepresentation.GetProperties().NoStaticSideEffects;
        }

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            if (ReadPurity(methodInterpreter))
                return;
            var functionIsPure = ComputeFunctionProperty(methodInterpreter);
            if (!functionIsPure) return;
            var additionalData = methodInterpreter.MidRepresentation.GetProperties();
            additionalData.NoStaticSideEffects = true;
            Result = true;
        }

        public static bool ComputeFunctionProperty(MethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.MidRepresentation.LocalOperations;
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
                    case OperationKind.SetStaticField:
                    case OperationKind.CallRuntime:
                    case OperationKind.SetField:
                        return false;

                    case OperationKind.Call:
                        var operationData = (MethodData)localOperation.Value;
                        var readPurity = LinkerInterpretersTableUtils.ReadNoStaticSideEffects(operationData.Info);
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