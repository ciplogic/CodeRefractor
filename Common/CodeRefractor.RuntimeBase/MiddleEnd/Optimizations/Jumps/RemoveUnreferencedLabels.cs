#region Usings

using System.Linq;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Jumps
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    public class RemoveUnreferencedLabels : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;

            var found = useDef.GetOperationsOfKind(OperationKind.Label).Length != 0;
            if (!found)
                return;

            var operationIndexes = useDef.GetOperationsOfKind(OperationKind.BranchOperator).ToList();
            operationIndexes.AddRange(useDef.GetOperationsOfKind(OperationKind.AlwaysBranch));
            var operations = useDef.GetLocalOperations();

            var candidateLabelTable = interpreter.MidRepresentation.UseDef.GetLabelTable(true);

            foreach (var index in operationIndexes)
            {
                var operation = operations[index];
                if (operation.Kind == OperationKind.BranchOperator)
                {
                    var destAssignment = (BranchOperator) operation;
                    candidateLabelTable.Remove(destAssignment.JumpTo);
                    continue;
                }
                if (operation.Kind == OperationKind.AlwaysBranch)
                {
                    candidateLabelTable.Remove(operation.Get<AlwaysBranch>().JumpTo);
                }
            }
            if (candidateLabelTable.Count == 0)
                return;
            var labelsToRemove = candidateLabelTable.Values.ToList();

            interpreter.DeleteInstructions(labelsToRemove);
            Result = true;
        }
    }
}