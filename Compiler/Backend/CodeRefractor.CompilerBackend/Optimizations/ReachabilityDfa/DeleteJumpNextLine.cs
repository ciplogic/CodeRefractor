#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ReachabilityDfa
{
    public class DeleteJumpNextLine : ResultingInFunctionOptimizationPass
    {
        private Dictionary<int, int> _labelTable;

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var operations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();

            var found = operations.Any(operation => operation.Kind == OperationKind.AlwaysBranch);
            if (!found)
                return;
            _labelTable = InstructionsUtils.BuildLabelTable(operations);
            var toRemove = new List<int>();
            for (var i = 0; i < operations.Length; i++)
            {
                var operation = operations[i];
                switch (operation.Kind)
                {
                    case OperationKind.AlwaysBranch:
                        var jumpLabel = JumpTo((int) operation.Value);
                        
                        if (jumpLabel != i + 1)
                            continue;
                        toRemove.Add(i);
                        return;
                    case OperationKind.BranchOperator:
                        
                        var destAssignment = (BranchOperator) operation.Value;
                        var jumpTo = JumpTo(destAssignment.JumpTo);
                        if (jumpTo != i + 1)
                            continue;
                        
                        toRemove.Add(i);
                        return;
                    default:
                        continue;
                }
            }
            if(toRemove.Count==0)
                return;
            methodInterpreter.DeleteInstructions(toRemove);
            Result = true;
                        
        }

        private int JumpTo(int labelId)
        {
            return _labelTable[labelId];
        }
    }
}