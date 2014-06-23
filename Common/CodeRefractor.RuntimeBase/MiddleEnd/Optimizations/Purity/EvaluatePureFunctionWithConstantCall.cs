#region Usings

using System.Collections.Generic;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
	[Optimization(Category = OptimizationCategories.Purity)]
    internal class EvaluatePureFunctionWithConstantCall : ResultingGlobalOptimizationPass
    {
        public override bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            var operations = midRepresentation.MidRepresentation.UseDef;

            return operations.GetOperationsOfKind(OperationKind.Call).Length > 0;
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var callIndices = useDef.GetOperationsOfKind(OperationKind.Call);
            foreach (var i in callIndices)
            {
                var operation = operations[i];
                var operationData = ComputeAndEvaluatePurityOfCall(operation);
                if (!operationData.Interpreter.AnalyzeProperties.IsPure || !operationData.Info.IsStatic)
                    continue;
                var methodInfo = operationData.Info;
                var constParams = new List<object>();
                var paramsAreConst = CheckIfParamAreConst(operationData, constParams);
                if (!paramsAreConst)
                    continue;
                var result = methodInfo.Invoke(null, constParams.ToArray());
                interpreter.MidRepresentation.LocalOperations[i] = new LocalOperation
                {
                    Value = new Assignment
                    {
                        AssignedTo = operationData.Result,
                        Right = new ConstValue(result)
                    }
                };
                Result = true;
            }
        }

        public static MethodData ComputeAndEvaluatePurityOfCall(LocalOperation operation)
        {
            var operationData = (MethodData) operation.Value;
            var methodInterpreter = operationData.Info.GetInterpreter(Runtime);
            if (AnalyzeFunctionPurity.ReadPurity(methodInterpreter))
            {
                operationData.Interpreter.AnalyzeProperties.IsPure = true;
            }
            else
            {
                if (methodInterpreter == null)
                {
                    return operationData;
                }
                if (methodInterpreter.Kind != MethodKind.Default)
                {
                    operationData.Interpreter.AnalyzeProperties.IsPure = false;
                    return operationData;
                }
                var computeIsPure = AnalyzeFunctionPurity.ComputeFunctionPurity(methodInterpreter);
                if (computeIsPure)
                    operationData.Interpreter.AnalyzeProperties.IsPure = true;
            }
            return operationData;
        }

        private static bool CheckIfParamAreConst(MethodData operationData, List<object> constParams)
        {
            var paramsAreConst = true;
            foreach (var parameter in operationData.Parameters)
            {
                var constParam = parameter as ConstValue;
                if (constParam != null)
                {
                    constParams.Add(constParam.Value);
                }
                else
                {
                    paramsAreConst = false;
                    break;
                }
            }
            return paramsAreConst;
        }
    }
}