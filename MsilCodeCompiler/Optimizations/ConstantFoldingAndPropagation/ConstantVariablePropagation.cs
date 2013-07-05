#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation
{
    public class ConstantVariablePropagation : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var operations = intermediateCode.LocalOperations;
            for (var i = 0; i < operations.Count - 1; i++)
            {
                Assignment srcVariableDefinition;
                var constValue = GetConstantFromOperation(operations, i, out srcVariableDefinition);
                if (constValue == null)
                    continue;
                for (var j = i + 1; j < operations.Count; j++)
                {
                    var destOperation = operations[j];
                    if (destOperation.Kind == LocalOperation.Kinds.Label)
                        break;
                    if (destOperation.Kind == LocalOperation.Kinds.BranchOperator)
                        break;
                    if (destOperation.Kind == LocalOperation.Kinds.Assignment)
                    {
                        var destAssignment = (Assignment) destOperation.Value;
                        if (SameVariable(destAssignment.Left, srcVariableDefinition.Left))
                            break;
                        if (!SameVariable(destAssignment.Right as LocalVariable, srcVariableDefinition.Left)) continue;
                        destAssignment.Right = constValue;
                        Result = true;
                    }
                    else if (destOperation.Kind == LocalOperation.Kinds.NewArray)
                    {
                        var destAssignment = (Assignment) destOperation.Value;
                        var arrayCreationInfo = (NewArrayObject) destAssignment.Right;
                        if (!SameVariable(arrayCreationInfo.ArrayLength as LocalVariable, srcVariableDefinition.Left))
                            continue;
                        arrayCreationInfo.ArrayLength = constValue;
                        Result = true;
                    }
                    else if (destOperation.Kind == LocalOperation.Kinds.SetField)
                    {
                        var destAssignment = (Assignment) destOperation.Value;
                        if (!SameVariable(destAssignment.Right as LocalVariable, srcVariableDefinition.Left)) continue;
                        destAssignment.Right = constValue;
                        Result = true;
                    }
                }
            }
        }
    }
}