#region Usings

using CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.Compiler.Optimizations
{
    public class ConstantVariableBranchOperatorPropagation : ConstantVariablePropagationBase
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
                    if (destOperation.Kind != LocalOperation.Kinds.BranchOperator) continue;
                    var destAssignment = (BranchOperator) destOperation.Value;
                    if (!SameVariable(destAssignment.CompareValue as LocalVariable, srcVariableDefinition.Left))
                        break;
                    Result = true;
                    var isTrue = ((int) constValue.Value != 0) ^ (destAssignment.Name != OpcodeBranchNames.BrTrue);
                    if (isTrue)
                    {
                        operations[j] = new LocalOperation
                                            {
                                                Kind = LocalOperation.Kinds.AlwaysBranch,
                                                Value = destAssignment.JumpTo
                                            };
                    }
                    else
                    {
                        operations.RemoveAt(j);
                    }
                    break;
                }
            }
        }
    }
}