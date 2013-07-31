#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.Compiler.Optimizations.ReachabilityDfa
{
    public class DeleteJumpNextLine : ResultingOptimizationPass
    {
        private Dictionary<int, int> _labelTable;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;


            var found = operations.Any(operation => operation.Kind == LocalOperation.Kinds.AlwaysBranch);
            if (!found)
                return;
            _labelTable = ReachabilityLines.BuildLabelTable(operations);
            for (var i = 0; i < operations.Count; i++)
            {
                var operation = operations[i];
                if (operation.Kind != LocalOperation.Kinds.AlwaysBranch)
                    continue;
                var jumpLabel = JumpTo((int) operation.Value);

                if (jumpLabel != i + 1) continue;
                Result = true;
                operations.RemoveAt(i);
            }
        }

        private int JumpTo(int labelId)
        {
            return _labelTable[labelId];
        }
    }
}