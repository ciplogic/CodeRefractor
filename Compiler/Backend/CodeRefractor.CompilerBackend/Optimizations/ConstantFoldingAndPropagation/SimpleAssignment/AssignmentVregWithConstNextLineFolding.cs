using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment
{
    public class AssignmentVregWithConstNextLineFolding : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;

            for (var index = 0; index < operations.Count-1; index++)
            {
                var localOperation = operations[index];
                if (localOperation.Kind != OperationKind.Assignment)
                    continue;

                var assignment = localOperation.GetAssignment();
                var constValue = assignment.Right ;
                var destOperation = operations[index + 1];
                if (!destOperation.OperationUses(assignment.AssignedTo)) continue;
                destOperation.SwitchUsageWithDefinition(assignment.AssignedTo, constValue);
                Result = true;
                break;
            }
        }
    }
}