#region Usings

using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation
{
    [Optimization(Category = OptimizationCategories.Propagation)]
    public class ConstantVariableOperatorPropagation : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var operations = interpreter.MidRepresentation.LocalOperations.ToArray();
            for (var i = 0; i < operations.Length - 1; i++)
            {
                Assignment srcVariableDefinition;
                var constValue = GetConstantFromOperation(operations[i], out srcVariableDefinition);
                if (constValue == null)
                    continue;

                for (var j = i + 1; j < operations.Length; j++)
                {
                    var destOperation = operations[j];
                    if (destOperation.Kind == OperationKind.Label)
                        break;
                    if (destOperation.Kind == OperationKind.BranchOperator)
                        break;
                    if (destOperation.Kind != OperationKind.BinaryOperator &&
                        destOperation.Kind != OperationKind.UnaryOperator) continue;
                    var destAssignment = (OperatorBase) destOperation;
                    if (SameVariable(destAssignment.AssignedTo, srcVariableDefinition.AssignedTo))
                        break;

                    var rightBinaryAssignment = destAssignment as BinaryOperator;
                    var unaryAssignment = destAssignment as UnaryOperator;

                    if (unaryAssignment != null) continue;
                    if (rightBinaryAssignment == null) continue;
                    if (SameVariable(rightBinaryAssignment.Left as LocalVariable, srcVariableDefinition.AssignedTo))
                    {
                        rightBinaryAssignment.Left = constValue;
                        Result = true;
                        continue;
                    }
                    if (SameVariable(rightBinaryAssignment.Right as LocalVariable, srcVariableDefinition.AssignedTo))
                    {
                        rightBinaryAssignment.Right = constValue;
                        Result = true;
                        continue;
                    }
                }
            }
        }
    }
}