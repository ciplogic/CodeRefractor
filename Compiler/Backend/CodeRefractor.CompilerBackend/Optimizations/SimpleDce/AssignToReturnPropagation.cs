#region Usings

using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    /// <summary>
    ///   This optimization in case of two assignments of the form: 
    /// > var1 = identifier 
    /// > var2 = var1 
    /// will transform the code to be 
    /// > var2 = identifier
    /// </summary>
    internal class AssignToReturnPropagation : ResultingInFunctionOptimizationPass
    {
        public override bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            var localOperations = midRepresentation.MidRepresentation.LocalOperations;
            return localOperations.Count >= 2;
        }
        public override void OptimizeOperations(MethodInterpreter intermediateCode)
        {
            var localOperations = intermediateCode.MidRepresentation.LocalOperations;
            var count = localOperations.Count;
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