#region Uses

using CodeRefractor.Backend.Linker;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Purity
{
    [Optimization(Category = OptimizationCategories.Analysis)]
    public class AnalyzeFunctionPurity : ResultingGlobalOptimizationPass
    {
        public static bool ReadPurity(MethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
            return intermediateCode.AnalyzeProperties.IsPure;
        }

        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            if (ReadPurity(interpreter))
                return;
            var functionIsPure = ComputeFunctionPurity(interpreter);
            if (!functionIsPure) return;

            if (interpreter.AnalyzeProperties.IsPure)
                return;
            interpreter.AnalyzeProperties.IsPure = true;
            Result = true;
        }

        public static bool ComputeFunctionPurity(CilMethodInterpreter intermediateCode)
        {
            if (intermediateCode == null)
                return false;
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
                        var operationData = (CallMethodStatic) localOperation;
                        var readPurity = LinkerInterpretersTableUtils.ReadPurity(operationData.Info, Closure);
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