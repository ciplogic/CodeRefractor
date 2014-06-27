#region Usings

using System.Collections.Generic;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.ReachabilityDfa
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class DeleteJumpNextLine : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var operations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var labelTable = interpreter.GetLabelTable();
            var toRemove = new List<int>();
            foreach (var labelInfo in labelTable)
            {
                var i = labelInfo.Value - 1;
                if (i < 0)
                    continue;
                var operation = operations[i];
                switch (operation.Kind)
                {
                    case OperationKind.AlwaysBranch:
                        var jumpLabel = labelTable[operation.Get<AlwaysBranch>().JumpTo];

                        if (jumpLabel != labelInfo.Value)
                            continue;
                        toRemove.Add(i);
                        continue;
                    case OperationKind.BranchOperator:

                        var destAssignment = (BranchOperator) operation;
                        var jumpTo = labelTable[destAssignment.JumpTo];
                        if (jumpTo != labelInfo.Value)
                            continue;

                        toRemove.Add(i);
                        continue;
                    default:
                        continue;
                }
            }
            if (toRemove.Count == 0)
                return;
            interpreter.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}