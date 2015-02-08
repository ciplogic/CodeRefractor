#region Usings

using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.Linker;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Purity
{
	[Optimization(Category = OptimizationCategories.Purity)]
    internal class EvaluatePureFunctionWithConstantCall : ResultingGlobalOptimizationPass
    {
        public override bool CheckPreconditions(CilMethodInterpreter midRepresentation, ClosureEntities closure)
        {
            var operations = midRepresentation.MidRepresentation.UseDef;

            return operations.GetOperationsOfKind(OperationKind.Call).Length > 0;
        }

        public override void OptimizeOperations(CilMethodInterpreter interpreter)
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
                interpreter.MidRepresentation.LocalOperations[i] = new Assignment
                    {
                        AssignedTo = operationData.Result,
                        Right = new ConstValue(result)
                    
                };
                Result = true;
            }
        }

        public static CallMethodStatic ComputeAndEvaluatePurityOfCall(LocalOperation operation)
        {
            var operationData = (CallMethodStatic) operation;
            var methodInterpreter = operationData.Info.GetInterpreter(Closure);
            if (AnalyzeFunctionPurity.ReadPurity(methodInterpreter as CilMethodInterpreter))
            {
                operationData.Interpreter.AnalyzeProperties.IsPure = true;
            }
            else
            {
                if (methodInterpreter == null)
                {
                    return operationData;
                }
                if (methodInterpreter.Kind != MethodKind.CilInstructions)
                {
                    operationData.Interpreter.AnalyzeProperties.IsPure = false;
                    return operationData;
                }
                var computeIsPure = AnalyzeFunctionPurity.ComputeFunctionPurity(methodInterpreter as CilMethodInterpreter);
                if (computeIsPure)
                    operationData.Interpreter.AnalyzeProperties.IsPure = true;
            }
            return operationData;
        }

        private static bool CheckIfParamAreConst(CallMethodStatic operationStatic, List<object> constParams)
        {
            var paramsAreConst = true;
            foreach (var parameter in operationStatic.Parameters)
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