#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation
{
    public class ConstantVariablePropagation : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
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
                    switch (destOperation.Kind)
                    {
                        case OperationKind.Assignment:
                        {
                            var destAssignment = (Assignment) destOperation.Value;
                            if (SameVariable(destAssignment.AssignedTo, srcVariableDefinition.AssignedTo))
                                break;
                            if (!SameVariable(destAssignment.Right as LocalVariable, srcVariableDefinition.AssignedTo))
                                continue;
                            destAssignment.Right = constValue;
                            Result = true;
                        }
                            break;
                        case OperationKind.NewArray:
                        {
                            var destAssignment = (Assignment) destOperation.Value;
                            var arrayCreationInfo = (NewArrayObject) destAssignment.Right;
                            if (
                                !SameVariable(arrayCreationInfo.ArrayLength as LocalVariable,
                                    srcVariableDefinition.AssignedTo))
                                continue;
                            arrayCreationInfo.ArrayLength = constValue;
                            Result = true;
                        }
                            break;
                        case OperationKind.SetField:
                        {
                            var destAssignment = (Assignment) destOperation.Value;
                            if (!SameVariable(destAssignment.Right as LocalVariable, srcVariableDefinition.AssignedTo))
                                continue;
                            destAssignment.Right = constValue;
                            Result = true;
                        }
                            break;
                    }
                }
            }
        }
    }
}