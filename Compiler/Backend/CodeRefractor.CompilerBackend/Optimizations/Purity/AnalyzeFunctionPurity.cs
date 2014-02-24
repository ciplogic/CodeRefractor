#region Usings

using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    public class AnalyzeFunctionPurity : ResultingGlobalOptimizationPass
    {
        public static bool ReadPurity(MethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            return intermediateCode.MidRepresentation.GetProperties().IsPure;
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            if (ReadPurity(interpreter))
                return;
            var functionIsPure = ComputeFunctionPurity(interpreter);
            if (!functionIsPure) return;
            interpreter.MidRepresentation.GetProperties().IsPure = true;
            Result = true;
        }

        public static bool ComputeFunctionPurity(MethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            intermediateCode.MidRepresentation.UpdateUseDef();
            var operations = intermediateCode.MidRepresentation.UseDef.GetLocalOperations();
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
                        var operationData = (MethodData) localOperation.Value;
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