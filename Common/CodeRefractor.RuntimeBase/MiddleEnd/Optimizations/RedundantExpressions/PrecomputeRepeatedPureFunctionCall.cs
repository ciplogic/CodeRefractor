#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.RedundantExpressions;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.Optimizations.RedundantExpressions
{
	[Optimization(Category = OptimizationCategories.CommonSubexpressionsElimination)]
    internal class PrecomputeRepeatedPureFunctionCall : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange,
            LocalOperation[] operations)
        {
            var localOperations = midRepresentation.MidRepresentation.LocalOperations;
            var calls = FindCallsToPureFunctions(midRepresentation.MidRepresentation.UseDef, startRange, endRange);
            if (calls.Count < 2)
                return false;
            for (var i = 0; i < calls.Count - 1; i++)
            {
                var firstMethodData = PrecomputeRepeatedUtils.GetMethodData(localOperations, calls, i);
                for (var j = i + 1; j < calls.Count; j++)
                {
                    var secondMethodData = PrecomputeRepeatedUtils.GetMethodData(localOperations, calls, j);
                    if (firstMethodData.Info != secondMethodData.Info)
                        continue;
                    var resultMerge = TryMergeCalls(i, j, firstMethodData, secondMethodData, localOperations);
                    if (!resultMerge) continue;
                    ApplyOptimization(midRepresentation, calls[i], calls[j]);
                    return true;
                }
            }
            return false;
        }

        private static void ApplyOptimization(MethodInterpreter midRepresentation, int i, int j)
        {
            var localOps = midRepresentation.MidRepresentation.LocalOperations;

            var srcMethod = PrecomputeRepeatedUtils.GetMethodData(localOps, i);
            var destMethod = PrecomputeRepeatedUtils.GetMethodData(localOps, j);
            if (srcMethod.Result == null)
                return;
            var computedType = srcMethod.Result.ComputedType();
            var newVreg = midRepresentation.CreateCacheVariable(computedType);

            var assignedTo = srcMethod.Result;
            var localOperation = PrecomputeRepeatedUtils.CreateAssignLocalOperation(assignedTo, newVreg);
            localOps.Insert(i + 1, localOperation);
            srcMethod.Result = newVreg;

            var destAssignment = PrecomputeRepeatedUtils.CreateAssignLocalOperation(destMethod.Result, newVreg);
            localOps.RemoveAt(j + 1);
            localOps.Insert(j + 1, destAssignment);
        }

        public static List<int> FindCallsToPureFunctions(UseDefDescription useDef, int startRange, int endRange)
        {
            var calls = new List<int>();
            var opArr = useDef.GetLocalOperations();
            var callIds = useDef.GetOperationsOfKind(OperationKind.Call);
            
            foreach (var index in callIds)
            {
                if(index < startRange || index > endRange)continue;
                var operation = opArr[index];
                var operationData = EvaluatePureFunctionWithConstantCall.ComputeAndEvaluatePurityOfCall(operation);
                if (!operationData.Interpreter.AnalyzeProperties.IsPure || !operationData.Info.IsStatic)
                    continue;
                calls.Add(index);
            }
            return calls;
        }

        private static bool TryMergeCalls(int i, int i1, MethodData firstMethodData, MethodData secondMethodData,
            List<LocalOperation> localOperations)
        {
            var validateParametersAreTheSame = ValidateParametersAreTheSame(firstMethodData, secondMethodData);
            if (!validateParametersAreTheSame)
                return false;
            return CheckReassignmentsOfParameters(i, i1, firstMethodData, localOperations);
        }

        private static bool CheckReassignmentsOfParameters(int i, int i1, MethodData firstMethodData,
            List<LocalOperation> localOperations)
        {
            var parametersFirst = new HashSet<LocalVariable>();
            foreach (var identifierValue in firstMethodData.Parameters)
            {
                var localVar = identifierValue as LocalVariable;
                if (localVar != null)
                    parametersFirst.Add(localVar);
            }

            for (var pos = i; pos <= i1; pos++)
            {
                var op = localOperations[pos];
                var definition = op.GetDefinition();
                if (definition == null)
                    continue;
                if (parametersFirst.Contains(definition))
                    return false;
            }
            return true;
        }

        private static bool ValidateParametersAreTheSame(MethodData firstMethodData, MethodData secondMethodData)
        {
            var parametersFirst = new List<IdentifierValue>();
            foreach (var identifierValue in firstMethodData.Parameters)
            {
                parametersFirst.Add(identifierValue);
            }
            var parametersSecond = new List<IdentifierValue>();
            foreach (var identifierValue in secondMethodData.Parameters)
            {
                parametersSecond.Add(identifierValue);
            }
            for (var index = 0; index < parametersFirst.Count; index++)
            {
                var id = parametersFirst[index];
                var id2 = parametersSecond[index];
                if (!id.Equals(id2))
                    return false;
            }
            return true;
        }
    }
}