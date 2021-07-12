#region Uses

using System.Collections.Generic;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.SimpleDce
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class DeleteJumpNextLine : OptimizationPassBase
    {
        public DeleteJumpNextLine()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var operations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var labelTable = interpreter.MidRepresentation.UseDef.GetLabelTable();
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
                return false;
            interpreter.DeleteInstructions(toRemove);
            return true;
        }
    }
}