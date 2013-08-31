using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class OneDefUsedPreviousLinePropagation : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOperations = intermediateCode.LocalOperations;
            for (var pos=0; pos< localOperations.Count-1; pos++)
            {
                var op = localOperations[pos];
                var nextOp = localOperations[pos+1];
                if (nextOp.Kind != LocalOperation.Kinds.Assignment)
                    continue;
                var nextAssignment = nextOp.GetAssignment();
                var nextVariable = nextAssignment.Right as LocalVariable;
                if(nextVariable==null || nextVariable.Kind != VariableKind.Vreg)
                    continue;

                var variableDefinition = op.GetUseDefinition();
                if (variableDefinition == null)
                    continue;
                if(variableDefinition.Kind!=VariableKind.Vreg)
                    continue;
                
                if(!variableDefinition.Equals(nextVariable))
                    continue;

                bool appliedOptimization = true;
                switch (op.Kind)
                {
                    case LocalOperation.Kinds.NewObject:
                    case LocalOperation.Kinds.Assignment:
                    case LocalOperation.Kinds.NewArray:
                        var newAssignment = op.GetAssignment();
                        newAssignment.AssignedTo = nextAssignment.AssignedTo;
                        break;
                    case LocalOperation.Kinds.BinaryOperator:
                    case LocalOperation.Kinds.UnaryOperator:
                        var newOperator = (OperatorBase)op.Value;
                        newOperator.AssignedTo = nextAssignment.AssignedTo;
                        break;
                    default:
                        appliedOptimization = false;
                        break;
                }
                if (appliedOptimization)
                {
                    Result = true;
                    localOperations.RemoveAt(pos+1);
                    return;
                }

            }

        }
    }
}