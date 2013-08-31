#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    /// <summary>
    ///   This optimization in case of two assignments of the form: > var1 = identifier > var2 = var1 will transform the code to be > var2 = identifier
    /// </summary>
    internal class DoubleAssignPropagation : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOperations = intermediateCode.LocalOperations;
            var count = localOperations.Count;
            for (var i = 0; i < count - 1; i++)
            {
                var firstInstruction = localOperations[i];
                if (firstInstruction.Kind != LocalOperation.Kinds.Assignment)
                    continue;
                var secondInstruction = localOperations[i + 1];
                if (secondInstruction.Kind != LocalOperation.Kinds.Assignment)
                    continue;
                var secondAssign = secondInstruction.GetAssignment();
                var localVariableSecondAssign = secondAssign.Right as LocalVariable;
                if (localVariableSecondAssign == null)
                    continue;
                var firstAssign = firstInstruction.GetAssignment();
                if (!localVariableSecondAssign.Equals(firstAssign.AssignedTo))
                    continue;
                if (secondAssign.Right == firstAssign.Right)
                    continue;
                secondAssign.Right = firstAssign.Right;
                Result = true;
            }
        }
    }
}