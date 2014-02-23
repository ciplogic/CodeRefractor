#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment
{
    internal class DeleteAssignmentWithSelf : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;

            var assigns = useDef.GetOperationsOfKind(OperationKind.Assignment);
            if (assigns.Length == 0)
                return;
            var localOps = useDef.GetLocalOperations();
            var toRemove = new List<int>();
            foreach (var index in assigns)
            {
                var localOp = localOps[index];
                if (localOp.Kind != OperationKind.Assignment)
                    continue;
                var assignment = localOp.GetAssignment();
                if (assignment.AssignedTo.Equals(assignment.Right))
                    toRemove.Add(index);
            }

            if (toRemove.Count == 0)
                return;
            midRepresentation.DeleteInstructions(toRemove);
            toRemove.Clear();
            Result = true;
        }
    }
}