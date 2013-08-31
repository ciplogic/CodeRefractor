#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ReachabilityDfa
{
    public class DeleteJumpNextLine : ResultingInFunctionOptimizationPass
    {
        private Dictionary<int, int> _labelTable;

        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;


            var found = operations.Any(operation => operation.Kind == LocalOperation.Kinds.AlwaysBranch);
            if (!found)
                return;
            _labelTable = InstructionsUtils.BuildLabelTable(operations);
            for (var i = 0; i < operations.Count; i++)
            {
                var operation = operations[i];
                switch (operation.Kind)
                {
                    case LocalOperation.Kinds.AlwaysBranch:
                        var jumpLabel = JumpTo((int) operation.Value);
                        
                        if (jumpLabel != i + 1)
                            continue;
                        
                        Result = true;
                        operations.RemoveAt(i);
                        return;
                    case LocalOperation.Kinds.BranchOperator:
                        
                        var destAssignment = (BranchOperator) operation.Value;
                        var jumpTo = JumpTo(destAssignment.JumpTo);
                        if (jumpTo != i + 1)
                            continue;
                        
                        Result = true;
                        operations.RemoveAt(i);
                        return;
                    default:
                        continue;
                }
            }
        }

        private int JumpTo(int labelId)
        {
            return _labelTable[labelId];
        }
    }
}