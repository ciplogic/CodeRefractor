#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation
{
    internal class EvaluatePureFunctionWithConstantCall : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            for (var i = 0; i < operations.Count - 1; i++)
            {
                var operation = operations[i];
                if (operation.Kind != LocalOperation.Kinds.Call)
                    continue;

                var operationData = (MethodData) operation.Value;
                if (!operationData.IsPure || !operationData.IsStatic)
                    continue;
                var methodInfo = operationData.Info;
                var constParams = new List<object>();
                var paramsAreConst = true;
                foreach (var parameter in operationData.Parameters)
                {
                    var constParam = parameter as ConstValue;
                    if (constParam == null)
                    {
                        paramsAreConst = false;
                        break;
                    }
                    constParams.Add(constParam.Value);
                }
                if (!paramsAreConst)
                    continue;
                var result = methodInfo.Invoke(null, constParams.ToArray());
                operations[i] = new LocalOperation
                                    {
                                        Kind = LocalOperation.Kinds.Assignment,
                                        Value = new Assignment
                                                    {
                                                        Left = operationData.Result,
                                                        Right = new ConstValue(result)
                                                    }
                                    };
            }
        }
    }
}