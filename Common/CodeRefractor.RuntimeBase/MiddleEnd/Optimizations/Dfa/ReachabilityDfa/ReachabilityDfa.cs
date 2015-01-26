#region Usings

using System.Collections.Generic;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Dfa.ReachabilityDfa
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class ReachabilityLines : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();

            var labelTable = useDef.GetLabelTable(true);
            var reached = new SortedSet<int>();
            Interpret(0, operations, labelTable, reached);
            if (reached.Count == operations.Length) return;
            Result = true;
            var toDelete = new List<int>();
            for (var i = 0; i < operations.Length; i++)
            {
                if (!reached.Contains(i))
                    toDelete.Add(i);
            }
            interpreter.DeleteInstructions(toDelete);
        }

        private void Interpret(int cursor, LocalOperation[] operations, Dictionary<int, int> labelTable,
            SortedSet<int> reached)
        {
            if (reached.Contains(cursor))
                return;
            var canUpdate = true;

            while (canUpdate)
            {
                reached.Add(cursor);
                var operation = operations[cursor];
                switch (operation.Kind)
                {
                    case OperationKind.BranchOperator:
                        var branchOperator = (BranchOperator) operation;
                        Interpret(labelTable[branchOperator.JumpTo], operations, labelTable, reached);
                        break;
                    case OperationKind.AlwaysBranch:
                        var jumpTo = ((AlwaysBranch) operation).JumpTo;
                        Interpret(labelTable[jumpTo], operations, labelTable, reached);
                        return;
                    case OperationKind.Switch:
                        var switchAssign = (Switch)operation;
                        var jumps = switchAssign.Jumps;
                        foreach (var jump in jumps)
                        {
                            Interpret(labelTable[jump], operations, labelTable, reached);
                        }
                        break;
                }
                cursor++;
                canUpdate = !reached.Contains(cursor) && cursor < operations.Length;
            }
        }
    }
}