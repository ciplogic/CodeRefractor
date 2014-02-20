#region Usings

using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Jumps
{
    public class RemoveUnreferencedLabels : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;

            var found = useDef.GetOperations(OperationKind.Label).Length != 0;
            if (!found)
                return;

            var operationIndexes = useDef.GetOperations(OperationKind.BranchOperator).ToList();
            operationIndexes.AddRange(useDef.GetOperations(OperationKind.AlwaysBranch));
            var operations = useDef.GetLocalOperations();

            var candidateLabelTable = interpreter.MidRepresentation.UseDef.GetLabelTable(true);

            foreach (var index in operationIndexes)
            {
                var operation = operations[index];
                if (operation.Kind == OperationKind.BranchOperator)
                {
                    var destAssignment = (BranchOperator) operation.Value;
                    candidateLabelTable.Remove(destAssignment.JumpTo);
                    continue;
                }
                if (operation.Kind == OperationKind.AlwaysBranch)
                {
                    candidateLabelTable.Remove((int) operation.Value);
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