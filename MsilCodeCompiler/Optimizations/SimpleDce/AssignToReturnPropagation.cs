using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.Compiler.Optimizations.SimpleDce
{
    /// <summary>
    /// This optimization in case of two assignments of the form:
    /// > var1 = identifier
    /// > var2 = var1
    /// will transform the code to be 
    /// > var2 = identifier
    /// </summary>
    internal class AssignToReturnPropagation : ResultingOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOperations = intermediateCode.LocalOperations;
            var count = localOperations.Count;

            var returnInstruction = localOperations[count - 1];
            if (returnInstruction.Kind != LocalOperation.Kinds.Return)
                return;
            var value = (IdentifierValue) returnInstruction.Value;
            if (value == null)
                return;
            var localVariableSecondAssign = value as LocalVariable;
            if (localVariableSecondAssign == null)
                return;

            var assignBeforeReturn = localOperations[count - 2];
            if (assignBeforeReturn.Kind != LocalOperation.Kinds.Assignment)
                return;

            var firstAssign = assignBeforeReturn.GetAssignment();
            if (!localVariableSecondAssign.Equals(firstAssign.Left))
                return;
            if (returnInstruction.Value == firstAssign.Right)
                return;
            returnInstruction.Value = firstAssign.Right;
            Result = true;
        }
    }
}