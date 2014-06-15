#region Usings

using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.SimpleDce
{
    /// <summary>
    ///     This optimization in case of two assignments of the form:
    ///     > var1 = identifier
    ///     > var2 = var1
    ///     will transform the code to be
    ///     > var2 = identifier
    /// </summary>
	[Optimization(Category = OptimizationCategories.DeadCodeElimination)]
    internal class AssignToReturnPropagation : ResultingInFunctionOptimizationPass
    {
        public override bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            var localOperations = midRepresentation.MidRepresentation.UseDef.GetLocalOperations();
            return localOperations.Length >= 2;
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var count = localOperations.Length;
            var assignBeforeReturn = localOperations[count - 2];
            if (assignBeforeReturn.Kind != OperationKind.Assignment)
                return;
            var returnInstruction = localOperations[count - 1];
            var value = (IdentifierValue) returnInstruction.Value;
            if (value == null)
                return;
            var localVariableSecondAssign = value as LocalVariable;
            if (localVariableSecondAssign == null)
                return;

            var firstAssign = assignBeforeReturn.GetAssignment();
            if (!localVariableSecondAssign.Equals(firstAssign.AssignedTo))
                return;
            if (returnInstruction.Value == firstAssign.Right)
                return;
            returnInstruction.Value = firstAssign.Right;
            Result = true;
        }
    }
}