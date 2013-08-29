using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.SimpleAssignment
{
    public class AssignmentWithVregPrevLineFolding : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;

            for (var index = 1; index < operations.Count - 1; index++)
            {
                var localOperation = operations[index];
                if (localOperation.Kind != LocalOperation.Kinds.Assignment)
                    continue;

                var assignment = localOperation.GetAssignment();
                var vregAssignment = assignment.Right as LocalVariable;
                
                if (vregAssignment==null || vregAssignment.Kind != VariableKind.Vreg) continue;
                
                var destOperation = operations[index - 1];
                var destOperationDefiniton = destOperation.GetUseDefinition();
                if (destOperationDefiniton == null || !destOperationDefiniton.Equals(vregAssignment)) continue;
                switch (destOperation.Kind)
                {
                    case LocalOperation.Kinds.UnaryOperator:
                    case LocalOperation.Kinds.BinaryOperator:
                        var operatorData = (OperatorBase)destOperation.Value;
                        operatorData.AssignedTo = assignment.AssignedTo;
                        break;
                    default:
                        continue;
                }
                Result = true;
                operations.RemoveAt(index);
                return;
            }
        }
    }
}