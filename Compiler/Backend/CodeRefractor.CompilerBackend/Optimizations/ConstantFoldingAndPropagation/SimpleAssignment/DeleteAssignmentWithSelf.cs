using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment
{
    class DeleteAssignmentWithSelf : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOps = intermediateCode.LocalOperations;
            var toRemove = new HashSet<int>();
            for (int index = 0; index < localOps.Count; index++)
            {
                var localOp = localOps[index];
                if(localOp.Kind!=OperationKind.Assignment)
                    continue;
                var assignment = localOp.GetAssignment();
                if(assignment.AssignedTo.Equals(assignment.Right))
                    toRemove.Add(index);
            }

            if (toRemove.Count == 0)
                return;
            intermediateCode.DeleteInstructions(toRemove);
            Result = true;
        }


    }
}