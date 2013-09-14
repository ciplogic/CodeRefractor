#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.CompilerBackend.Linker;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    internal class EvaluatePureFunctionWithConstantCall : ResultingGlobalOptimizationPass
    {
        public override bool CheckPreconditions(MetaMidRepresentation midRepresentation)
        {
            var operations = midRepresentation.LocalOperations;

            return operations.Any(op => op.Kind == OperationKind.Call);
        }

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            for (var i = 0; i < operations.Count - 1; i++)
            {
                var operation = operations[i];
                if (operation.Kind != OperationKind.Call)
                    continue;

                var operationData = (MethodData)operation.Value;
                var methodInterpreter = LinkerUtils.GetMethod(operationData.Info);
                if (AnalyzeFunctionPurity.ReadPurity(methodInterpreter))
                {
                    operationData.IsPure = true;
                }
                else
                {
                    var computeIsPure = AnalyzeFunctionPurity.ComputeFunctionPurity(methodInterpreter);
                    if (computeIsPure)
                        operationData.IsPure = true;
                }
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