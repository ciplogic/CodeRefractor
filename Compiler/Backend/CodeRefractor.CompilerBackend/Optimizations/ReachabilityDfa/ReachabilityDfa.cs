#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ReachabilityDfa
{
    public class ReachabilityLines : ResultingInFunctionOptimizationPass
    {
        private Dictionary<int, int> _labelTable;
        private HashSet<int> _reached;

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var operations = methodInterpreter.MidRepresentation.LocalOperations;
            _labelTable = InstructionsUtils.BuildLabelTable(operations);
            _reached = new HashSet<int>();
            Interpret(0, operations);
            if (_reached.Count == operations.Count) return;
            Result = true;
            var toDelete = new List<int>();
            for (var i = 0; i < operations.Count; i++)
            {
                if (!_reached.Contains(i))
                    toDelete.Add(i);
            }
            toDelete.Reverse();
            foreach (var i in toDelete)
            {
                operations.RemoveAt(i);
            }
        }

        private int JumpTo(int labelId)
        {
            return _labelTable[labelId];
        }

        private void Interpret(int cursor, List<LocalOperation> operations)
        {
            if (_reached.Contains(cursor))
                return;
            var canUpdate = true;

            while (canUpdate)
            {
                _reached.Add(cursor);
                var operation = operations[cursor];
                switch (operation.Kind)
                {
                    case OperationKind.BranchOperator:
                        var branchOperator = (BranchOperator) operation.Value;
                        Interpret(JumpTo(branchOperator.JumpTo), operations);
                        break;
                    case OperationKind.AlwaysBranch:
                        var jumpTo = (int) operation.Value;
                        Interpret(JumpTo(jumpTo), operations);
                        return;
                        case OperationKind.Switch:
                        var switchAssign = operation.GetAssignment();
                        var jumps = (int[])((ConstValue)switchAssign.Right).Value;
                        foreach(var jump in jumps)
                        {
                            Interpret(JumpTo(jump), operations);
                        }
                        break;
                }
                cursor++;
                canUpdate = !_reached.Contains(cursor) && cursor < operations.Count;
            }
        }
    }
}