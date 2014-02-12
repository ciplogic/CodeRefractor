#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    internal class EvaluatePureFunctionWithConstantCall : ResultingGlobalOptimizationPass
    {
        public override bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            var operations = midRepresentation.MidRepresentation.UseDef;

            return operations.GetOperations(OperationKind.Call).Length > 0;
        }

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            var callIndices = useDef.GetOperations(OperationKind.Call);
            foreach (var i in callIndices)
            {
                var operation = operations[i];
                var operationData = ComputeAndEvaluatePurityOfCall(operation);
                if (!operationData.IsPure || !operationData.IsStatic)
                    continue;
                var methodInfo = operationData.Info;
                var constParams = new List<object>();
                var paramsAreConst = CheckIfParamAreConst(operationData, constParams);
                if (!paramsAreConst)
                    continue;
                var result = methodInfo.Invoke(null, constParams.ToArray());
                operations[i] = new LocalOperation
                {
                    Kind = OperationKind.Assignment,
                    Value = new Assignment
                    {
                        AssignedTo = operationData.Result,
                        Right = new ConstValue(result)
                    }
                };
            }
        }

        public static MethodData ComputeAndEvaluatePurityOfCall(LocalOperation operation)
        {
            var operationData = (MethodData) operation.Value;
            var methodInterpreter = operationData.Info.GetInterpreter();
            if (AnalyzeFunctionPurity.ReadPurity(methodInterpreter))
            {
                operationData.IsPure = true;
            }
            else
            {
                if (methodInterpreter == null)
                {
                    return operationData;
                }
                if (methodInterpreter.Kind != MethodKind.Default)
                {
                    operationData.IsPure = false;
                    return operationData;
                }
                var computeIsPure = AnalyzeFunctionPurity.ComputeFunctionPurity(methodInterpreter);
                if (computeIsPure)
                    operationData.IsPure = true;
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