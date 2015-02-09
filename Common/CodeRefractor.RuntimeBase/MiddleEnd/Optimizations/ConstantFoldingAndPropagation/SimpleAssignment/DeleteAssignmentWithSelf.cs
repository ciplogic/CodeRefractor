#region Usings

using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment
{
    [Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    internal class DeleteAssignmentWithSelf  : OptimizationPassBase
    {
        public DeleteAssignmentWithSelf()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;

            var assigns = useDef.GetOperationsOfKind(OperationKind.Assignment);
            if (assigns.Length == 0)
                return false;
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
                return false;
            midRepresentation.DeleteInstructions(toRemove);
            toRemove.Clear();
            return true;
        }
    }
}