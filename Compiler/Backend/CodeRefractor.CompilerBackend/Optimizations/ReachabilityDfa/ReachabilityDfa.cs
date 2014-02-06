#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ReachabilityDfa
{
    public class ReachabilityLines : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();
            
            var labelTable =useDef.GetLabelTable(true);
            var reached = new SortedSet<int>();
            Interpret(0, operations,labelTable, reached);
            if (reached.Count == operations.Length) return;
            Result = true;
            var toDelete = new List<int>();
            for (var i = 0; i < operations.Length; i++)
            {
                if (!reached.Contains(i))
                    toDelete.Add(i);
            }
            methodInterpreter.DeleteInstructions(toDelete);
        }

        private void Interpret(int cursor, LocalOperation[] operations, Dictionary<int, int> labelTable, SortedSet<int> reached)
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
                        var branchOperator = (BranchOperator) operation.Value;
                        Interpret(labelTable[branchOperator.JumpTo], operations,labelTable,reached);
                        break;
                    case OperationKind.AlwaysBranch:
                        var jumpTo = (int) operation.Value;
                        Interpret(labelTable[jumpTo], operations, labelTable, reached);
                        return;
                        case OperationKind.Switch:
                        var switchAssign = operation.GetAssignment();
                        var jumps = (int[])((ConstValue)switchAssign.Right).Value;
                        foreach(var jump in jumps)
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