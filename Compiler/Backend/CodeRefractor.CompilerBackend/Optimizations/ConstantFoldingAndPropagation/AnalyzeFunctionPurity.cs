using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.CompilerBackend.Linker;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation
{
    public class AnalyzeFunctionPurity : ResultingOptimizationPass
    {
        public const string IsPureString = "IsPure";

        public static bool ReadPurity(MetaMidRepresentation intermediateCode)
        {
            object isPureData;
            var additionalData = intermediateCode.AuxiliaryObjects;
            return additionalData.TryGetValue(IsPureString, out isPureData);
        }
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            if (ReadPurity(intermediateCode))
                return;
            var operations = intermediateCode.LocalOperations;
            var functionIsPure = ComputeFunctionPurity(operations);
            var additionalData = intermediateCode.AuxiliaryObjects;
            if (!functionIsPure) return;
            additionalData[IsPureString] = true;
            Result = true;
        }

        public static bool ComputeFunctionPurity(List<LocalOperation> operations)
        {
            foreach (var localOperation in operations)
            {
                switch (localOperation.Kind)
                {
                    case LocalOperation.Kinds.SetStaticField:
                    case LocalOperation.Kinds.GetStaticField:
                    case LocalOperation.Kinds.CallRuntime:

                    case LocalOperation.Kinds.SetField:
                        return false;
                        
                    case LocalOperation.Kinds.Call:

                        var operationData = (MethodData)localOperation.Value;
                        var methodInterpreter = LinkerInterpretersTable.GetMethod(operationData.Info);
                        if (!ReadPurity(methodInterpreter))
                            return false;
                        break;

                    case LocalOperation.Kinds.BranchOperator:
                    case LocalOperation.Kinds.AlwaysBranch:
                    case LocalOperation.Kinds.UnaryOperator:
                    case LocalOperation.Kinds.BinaryOperator:
                    case LocalOperation.Kinds.Assignment:
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