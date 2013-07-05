#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.Compiler.Optimizations
{
    public class RemoveUnreferencedLabels : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var candidateLabelTable = new Dictionary<int, int>();
            var operations = intermediateCode.LocalOperations;
            var pos = -1;
            foreach (var operation in operations)
            {
                pos++;
                if (operation.Kind == LocalOperation.Kinds.Label)
                    candidateLabelTable[(int) operation.Value] = pos;
            }

            foreach (var operation in operations)
            {
                if (operation.Kind == LocalOperation.Kinds.BranchOperator)
                {
                    var destAssignment = (BranchOperator) operation.Value;
                    candidateLabelTable.Remove(destAssignment.JumpTo);
                    continue;
                }
                if (operation.Kind == LocalOperation.Kinds.AlwaysBranch)
                {
                    candidateLabelTable.Remove((int) operation.Value);
                }
            }
            if (candidateLabelTable.Count == 0)
                return;
            var labelsToRemove = candidateLabelTable.Values.OrderBy(v => v).Reverse().ToList();
            foreach (var operationPos in labelsToRemove)
            {
                operations.RemoveAt(operationPos);
            }
            Result = true;
        }
    }
}