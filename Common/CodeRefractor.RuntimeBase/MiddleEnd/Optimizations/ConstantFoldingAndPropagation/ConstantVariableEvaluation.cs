#region Uses

using System;
using System.IO;
using System.Linq;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;
using CodeRefractor.Shared;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation
{
    [Optimization(Category = OptimizationCategories.Constants)]
    public class ConstantVariableEvaluation : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var operations = interpreter.MidRepresentation.LocalOperations
                .Where(op => op.Kind == OperationKind.BranchOperator)
                .Select(operation => (BranchOperator) operation)
                .ToArray();
            foreach (var operation in operations)
            {
                var leftVal = (operation.CompareValue as ConstValue);
                var rightVal = (operation.SecondValue as ConstValue);
                if (leftVal == null || rightVal == null)
                    continue;
                switch (operation.Name)
                {
                    case OpcodeBranchNames.Beq:
                        FoldEvaluation(operation, EvaluateBeq(leftVal, rightVal));
                        break;
                    default:
                        throw new InvalidDataException(
                            "ConstantVariableEvaluation optimization. Case not handled, report a bug with reduced code");
                }
            }
        }

        private void FoldEvaluation(BranchOperator operation, bool resultEq)
        {
            operation.CompareValue = new ConstValue(resultEq ? 1 : 0);
            operation.Name = OpcodeBranchNames.BrTrue;
            Result = true;
        }

        private static bool EvaluateBeq(ConstValue leftVal, ConstValue rightVal)
        {
            var clrTypeCode = leftVal.ComputedType().ClrTypeCode;
            switch (clrTypeCode)
            {
                case TypeCode.Int32:
                    return (int)leftVal.Value == (int)rightVal.Value;
                default:
                    throw new InvalidDataException(
                        "ConstantVariableEvaluation optimization. Case not handled, report a bug with reduced code");
            }
        }
    }
}