#region Uses

using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.Optimizations;

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
class AssignToReturnPropagation : OptimizationPassBase
    {
        public AssignToReturnPropagation()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            if (localOperations.Length < 2)
                return false;
            var count = localOperations.Length;
            var assignBeforeReturn = localOperations[count - 2];
            if (assignBeforeReturn.Kind != OperationKind.Assignment)
                return false;
            var returnInstruction = localOperations[count - 1].Get<Return>();
            var value = returnInstruction.Returning;
            if (value == null)
                return false;
            var localVariableSecondAssign = value as LocalVariable;
            if (localVariableSecondAssign == null)
                return false;

            var firstAssign = assignBeforeReturn.GetAssignment();
            if (!localVariableSecondAssign.Equals(firstAssign.AssignedTo))
                return false;
            if (returnInstruction.Returning.Equals(firstAssign.Right))
                return false;
            returnInstruction.Returning = firstAssign.Right;
            return true;
        }
    }
}