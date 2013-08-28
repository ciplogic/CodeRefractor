#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations
{
    public class VRegVariablePropagation : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            for (var i = 0; i < intermediateCode.LocalOperations.Count - 1; i++)
            {
                var srcOperation = intermediateCode.LocalOperations[i];
                if (srcOperation.Kind != LocalOperation.Kinds.Assignment
                    && srcOperation.Kind != LocalOperation.Kinds.NewObject) continue;

                var srcVariableDefinition = srcOperation.Value as Assignment;
                if (srcVariableDefinition == null)
                    continue;
                var srcValue = srcVariableDefinition.Right;
                var srcVariable = srcValue as LocalVariable;

                var assignValue = ComputeAssignedValue(srcValue);

                var destOperation = intermediateCode.LocalOperations[i + 1];
                if (!(destOperation.Kind == LocalOperation.Kinds.BinaryOperator ||
                      destOperation.Kind == LocalOperation.Kinds.UnaryOperator) ||
                    destOperation.Kind == LocalOperation.Kinds.Assignment)
                    continue;
                if (destOperation.Kind == LocalOperation.Kinds.BinaryOperator)
                {
                    var binaryOperator = destOperation.Value as BinaryOperator;
                    if (binaryOperator != null)
                    {
                        if (TryFoldBinary(binaryOperator, srcVariableDefinition.AssignedTo.Id, srcValue))
                        {
                            intermediateCode.LocalOperations.RemoveAt(i);
                            Result = true;
                            continue;
                        }
                        continue;
                    }
                }
                if (destOperation.Kind == LocalOperation.Kinds.UnaryOperator)
                {
                    var unaryOperator = destOperation.Value as UnaryOperator;
                    if (unaryOperator != null)
                    {
                        if (TryFoldUnary(unaryOperator, srcVariableDefinition.AssignedTo.Id, srcVariable))
                        {
                            intermediateCode.LocalOperations.RemoveAt(i);
                            Result = true;
                            continue;
                        }
                        continue;
                    }
                    continue;
                }
                if (destOperation.Kind == LocalOperation.Kinds.Assignment)
                {
                    var destVariable = (Assignment) destOperation.Value;
                    var vregVar = destVariable.Right as LocalVariable;
                    if (vregVar == null)
                        continue;
                    if (vregVar.Kind != VariableKind.Vreg || vregVar.Id != srcVariableDefinition.AssignedTo.Id)
                        continue;
                    destVariable.Right = assignValue;
                    intermediateCode.LocalOperations.RemoveAt(i);
                    Result = true;
                }
            }
        }

        private bool TryFoldBranch(BranchOperator branchOperator, int id, LocalVariable srcVariable)
        {
            var leftField = branchOperator.CompareValue as LocalVariable;
            if (leftField == null)
                return false;
            if (leftField.Id != id || leftField.Kind != VariableKind.Vreg)
                return false;
            branchOperator.CompareValue = srcVariable;
            return true;
        }

        private static bool TryFoldUnary(UnaryOperator unaryOperator, int id, LocalVariable srcVariable)
        {
            var leftField = unaryOperator.Left as LocalVariable;
            if (leftField == null) return false;
            if (leftField.Id != id || leftField.Kind != VariableKind.Vreg)
                return false;
            unaryOperator.Left = srcVariable;
            return true;
        }

        private static bool TryFoldBinary(BinaryOperator binaryOperator, int id, object srcVariable)
        {
            var leftField = binaryOperator.Left as LocalVariable;
            var rightField = binaryOperator.Right as LocalVariable;
            var assignValue = ComputeAssignedValue(srcVariable);
            if (leftField != null)
            {
                if (leftField.Id == id && leftField.Kind == VariableKind.Vreg)
                {
                    binaryOperator.Left = assignValue;
                    return true;
                }
            }
            if (rightField != null)
            {
                if (rightField.Id == id && rightField.Kind == VariableKind.Vreg)
                {
                    binaryOperator.Right = assignValue;
                    return true;
                }
            }
            return false;
        }

        private static IdentifierValue ComputeAssignedValue(object srcVariable)
        {
            var destValue = srcVariable as LocalVariable;
            ConstValue constVal = null;
            if (destValue == null)
            {
                constVal = srcVariable as ConstValue;
            }
            var assignValue = destValue != null ? (IdentifierValue) destValue : constVal;
            return assignValue;
        }
    }
}