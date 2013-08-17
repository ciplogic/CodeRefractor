#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation
{
    internal class EvaluatePureFunctionWithConstantCall : ResultingOptimizationPass
    {
        public override bool CheckPreconditions(MetaMidRepresentation midRepresentation)
        {
            var operations = midRepresentation.LocalOperations;

            return operations.Any(op => op.Kind == LocalOperation.Kinds.Call);
        }

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            for (var i = 0; i < operations.Count - 1; i++)
            {
                var operation = operations[i];
                if (operation.Kind != LocalOperation.Kinds.Call)
                    continue;

                var operationData = (MethodData)operation.Value;
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
                    Kind = LocalOperation.Kinds.Assignment,
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