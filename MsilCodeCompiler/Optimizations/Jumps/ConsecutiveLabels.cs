#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.Compiler.Optimizations.Jumps
{
    internal class ConsecutiveLabels : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;

            var found = operations.Any(operation => operation.Kind == LocalOperation.Kinds.Label);
            if (!found)
                return;
            for (var i = 0; i < operations.Count - 2; i++)
            {
                var operation = operations[i];
                if (operation.Kind != LocalOperation.Kinds.Label)
                    continue;

                var operation2 = operations[i + 1];
                if (operation2.Kind != LocalOperation.Kinds.Label)
                    continue;
                var jumpId = (int) operation.Value;
                var jumpId2 = (int) operation2.Value;
                OptimizeConsecutiveLabels(operations, jumpId, jumpId2);
                operations.RemoveAt(i + 1);
                Result = true;
            }
        }

        private void OptimizeConsecutiveLabels(List<LocalOperation> operations, int jumpId, int jumpId2)
        {
            for (var i = 0; i < operations.Count - 2; i++)
            {
                var operation = operations[i];
                if (operation.IsBranchOperation())
                    continue;
                switch (operation.Kind)
                {
                    case LocalOperation.Kinds.AlwaysBranch:
                        var jumpTo = (int) operation.Value;
                        if (jumpId2 == jumpTo)
                            operation.Value = jumpId;
                        break;
                    case LocalOperation.Kinds.BranchOperator:
                        var destAssignment = (BranchOperator) operation.Value;
                        if (destAssignment.JumpTo == jumpId2)
                            destAssignment.JumpTo = jumpId;
                        break;
                }
            }
        }
    }
}