using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.Purity
{
    class PrecomputeRepeatedPureFunctionCall : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MetaMidRepresentation midRepresentation, int startRange, int endRange)
        {
            var localOperations = midRepresentation.LocalOperations;
            var calls = FindCallsToPureFunctions(localOperations, startRange, endRange);
            if (calls.Count < 2)
                return false;
            for(var i = 0;i<calls.Count-1;i++)
            {
                var firstMethodData = GetMethodData(localOperations, calls, i);
                for (int j = i+1; j < calls.Count; j++)
                {
                    var secondMethodData = GetMethodData(localOperations, calls, j);
                    if(firstMethodData.Info!=secondMethodData.Info)
                        continue;
                    var resultMerge = TryMergeCalls(i, j, firstMethodData, secondMethodData, localOperations);
                    if (resultMerge)
                    {
                        ApplyOptimization(midRepresentation, calls[i], calls[j]);
                        return true;
                    }
                }
            }
            return false;
        }

        private static void ApplyOptimization(MetaMidRepresentation midRepresentation, int i, int j)
        {
            var max = midRepresentation.Vars.VirtRegs.Max(vreg => vreg.Id) + 1;
            var localOps = midRepresentation.LocalOperations;

            var srcMethod = GetMethodData(localOps, i);
            var destMethod = GetMethodData(localOps, j);

            var cacheVariable = new LocalVariable()
            {
                FixedType = srcMethod.Result.ComputedType(),
                Id = max,
                Kind = VariableKind.Vreg
            };

            midRepresentation.Vars.VirtRegs.Add(cacheVariable);
            
            var assignment = new Assignment
                {
                    AssignedTo = (LocalVariable) srcMethod.Result.Clone(),
                    Right = cacheVariable.Clone()
                };
            localOps.Insert(i+1, new LocalOperation()
                {
                    Kind = OperationKind.Assignment,
                    Value = assignment
                });
            srcMethod.Result = cacheVariable;

            var destAssignment = new Assignment
            {
                AssignedTo = (LocalVariable) destMethod.Result.Clone(),
                Right = cacheVariable.Clone()
            };
            localOps.RemoveAt(j+1);
            localOps.Insert(j + 1, new LocalOperation()
            {
                Kind = OperationKind.Assignment,
                Value = destAssignment
            });
       
        }

        public static List<int> FindCallsToPureFunctions(List<LocalOperation> localOperations, int startRange, int endRange)
        {
            var calls = new List<int>();
            for (var index = startRange; index <= endRange; index++)
            {
                var operation = localOperations[index];
                if (operation.Kind != OperationKind.Call)
                    continue;
                var operationData = EvaluatePureFunctionWithConstantCall.ComputeAndEvaluatePurityOfCall(operation);
                if (!operationData.IsPure || !operationData.IsStatic)
                    continue;
                calls.Add(index);
            }
            return calls;
        }

        private bool TryMergeCalls(int i, int i1, MethodData firstMethodData, MethodData secondMethodData, List<LocalOperation> localOperations)
        {
            var validateParametersAreTheSame = ValidateParametersAreTheSame(firstMethodData, secondMethodData);
            if(!validateParametersAreTheSame)
                return false;
            return CheckReassignmentsOfParameters(i, i1, firstMethodData, localOperations);
        }

        private static bool CheckReassignmentsOfParameters(int i, int i1, MethodData firstMethodData, List<LocalOperation> localOperations)
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
                var definition = op.GetUseDefinition();
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

        private static MethodData GetMethodData(List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetMethodData(localOperations, index);
        }

        public static MethodData GetMethodData(List<LocalOperation> localOperations, int index)
        {
            return (MethodData) localOperations[index].Value;
        }
    }
}