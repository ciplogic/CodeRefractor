#region Usings

using System;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation
{
    public class ConstantVariableBranchOperatorPropagation : ConstantVariablePropagationBase
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var operations = interpreter.MidRepresentation.LocalOperations;
            for (var i = 0; i < operations.Count - 1; i++)
            {
                var destOperation = operations[i];
                if (destOperation.Kind != OperationKind.BranchOperator) continue;
                var destAssignment = (BranchOperator) destOperation.Value;
                if (destAssignment.Name != OpcodeBranchNames.BrTrue && destAssignment.Name != OpcodeBranchNames.BrFalse)
                    continue;
                var constValue = destAssignment.CompareValue as ConstValue;
                if (constValue == null)
                    continue;
                Result = true;
                var expressionValue = Convert.ToInt32(constValue.Value) != 0;
                var isTrue = (expressionValue) ^ (destAssignment.Name != OpcodeBranchNames.BrTrue);
                if (isTrue)
                {
                    operations[i] = new LocalOperation
                    {
                        Kind = OperationKind.AlwaysBranch,
                        Value = destAssignment.JumpTo
                    };
                }
                else
                {
                    operations.RemoveAt(i);
                }
                return;
            }
        }
    }
}