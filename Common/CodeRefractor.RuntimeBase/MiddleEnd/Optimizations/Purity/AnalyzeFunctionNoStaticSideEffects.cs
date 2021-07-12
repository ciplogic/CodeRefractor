#region Uses

using CodeRefractor.Backend.Linker;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Purity
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    public class AnalyzeFunctionNoStaticSideEffects : ResultingGlobalOptimizationPass
    {
        public static bool ReadPurity(CilMethodInterpreter intermediateCode)
        {
            return intermediateCode.AnalyzeProperties.IsReadOnly;
        }

        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            if (ReadPurity(interpreter))
                return;
            var functionIsPure = ComputeFunctionProperty(interpreter);
            if (!functionIsPure) return;
            var additionalData = interpreter.AnalyzeProperties;
            additionalData.IsReadOnly = true;
            Result = true;
        }

        public static bool ComputeFunctionProperty(CilMethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            var operations = intermediateCode.MidRepresentation.UseDef.GetLocalOperations();
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
                    case OperationKind.SetStaticField:
                    case OperationKind.CallRuntime:
                    case OperationKind.SetField:
                        return false;

                    case OperationKind.Call:
                        var operationData = (CallMethodStatic) localOperation;
                        var readPurity = LinkerInterpretersTableUtils.ReadNoStaticSideEffects(operationData.Info,
                            Closure);
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