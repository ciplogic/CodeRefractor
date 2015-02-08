#region Usings

using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.SimpleDce
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
        public override bool CheckPreconditions(CilMethodInterpreter midRepresentation, ClosureEntities closure)
        {
            var localOperations = midRepresentation.MidRepresentation.UseDef.GetLocalOperations();
            return localOperations.Length >= 2;
        }

        public override void OptimizeOperations(CilMethodInterpreter interpreter)
        {
            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var count = localOperations.Length;
            var assignBeforeReturn = localOperations[count - 2];
            if (assignBeforeReturn.Kind != OperationKind.Assignment)
                return;
            var returnInstruction = localOperations[count - 1].Get<Return>();
            var value = returnInstruction.Returning;
            if (value == null)
                return;
            var localVariableSecondAssign = value as LocalVariable;
            if (localVariableSecondAssign == null)
                return;

            var firstAssign = assignBeforeReturn.GetAssignment();
            if (!localVariableSecondAssign.Equals(firstAssign.AssignedTo))
                return;
            if (returnInstruction.Returning.Equals(firstAssign.Right))
                return;
            returnInstruction.Returning= firstAssign.Right;
            Result = true;
        }
    }
}