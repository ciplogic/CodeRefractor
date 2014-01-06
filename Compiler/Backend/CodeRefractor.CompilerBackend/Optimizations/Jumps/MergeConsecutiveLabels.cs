#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Jumps
{
    internal class MergeConsecutiveLabels : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter intermediateCode)
        {
            var operations = intermediateCode.MidRepresentation.LocalOperations;

            var found = operations.Any(operation => operation.Kind == OperationKind.Label);
            if (!found)
                return;
            for (var i = 0; i < operations.Count - 2; i++)
            {
                var operation = operations[i];
                if (operation.Kind != OperationKind.Label)
                    continue;

                var operation2 = operations[i + 1];
                if (operation2.Kind != OperationKind.Label)
                    continue;
                var jumpId = (int) operation.Value;
                var jumpId2 = (int) operation2.Value;
                OptimizeConsecutiveLabels(operations, jumpId, jumpId2);
                operations.RemoveAt(i + 1);
                Result = true;
            }
        }

        private static void OptimizeConsecutiveLabels(List<LocalOperation> operations, int jumpId, int jumpId2)
        {
            for (var i = 0; i < operations.Count - 2; i++)
            {
                var operation = operations[i];
                if (!operation.IsBranchOperation())
                    continue;
                switch (operation.Kind)
                {
                    case OperationKind.AlwaysBranch:
                        var jumpTo = (int) operation.Value;
                        if (jumpId2 == jumpTo)
                            operation.Value = jumpId;
                        break;
                    case OperationKind.BranchOperator:
                        var destAssignment = (BranchOperator) operation.Value;
                        if (destAssignment.JumpTo == jumpId2)
                            destAssignment.JumpTo = jumpId;
                        break;
                }
            }
        }
    }
}