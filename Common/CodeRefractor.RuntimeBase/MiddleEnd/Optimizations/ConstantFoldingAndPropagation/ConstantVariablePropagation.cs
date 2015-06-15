#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation
{
    [Optimization(Category = OptimizationCategories.Constants)]
    public class ConstantVariablePropagation : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var operations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
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
                            var destAssignment = (Assignment) destOperation;
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
                            var destAssignment = (NewArrayObject) destOperation;
                            var arrayCreationInfo = destAssignment;
                            if (!SameVariable(arrayCreationInfo.ArrayLength as LocalVariable,
                                srcVariableDefinition.AssignedTo))
                                continue;
                            arrayCreationInfo.ArrayLength = constValue;
                            Result = true;
                        }
                            break;
                        case OperationKind.SetField:
                        {
                            var destAssignment = (Assignment) destOperation;
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