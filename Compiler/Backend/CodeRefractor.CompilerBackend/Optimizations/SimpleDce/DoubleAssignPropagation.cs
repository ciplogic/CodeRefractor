#region Usings

using System.Collections.Generic;
using System.Linq;
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
    ///  will transform the code to be > var2 = identifier
    /// </summary>
    internal class AssignBackDcePropagation : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOperations = intermediateCode.LocalOperations;
            var count = localOperations.Count;
            for (var i = 0; i < localOperations.Count-1; i++)
            {
          
                var secondInstruction = localOperations[i + 1];
                if (secondInstruction.Kind != OperationKind.Assignment)
                    continue;
                var secondAssign = secondInstruction.GetAssignment();
                var localVariableSecondAssign = secondAssign.Right as LocalVariable;
                if (localVariableSecondAssign == null)
                    continue;

                var defs = UseDefHelper.GetVariableDefinitions(intermediateCode, localVariableSecondAssign);
                if(defs.Count>1)
                    continue;
                if (defs.Count == 0)
                {
                    continue;
                }
                if (defs.First() != i)
                {
                    continue;
                }
                var replaceVariable = secondAssign.AssignedTo;

                var usages = UseDefHelper.GetUsagesAndDefinitions(intermediateCode, localVariableSecondAssign);
                if(usages.Count>2)
                    continue;
                localOperations = intermediateCode.LocalOperations;
                foreach (var usage in usages)
                {
                    var prevInstruction = localOperations[usage];
                    UseDefHelper.SwitchUsageWithDefinition(
                        prevInstruction,
                        localVariableSecondAssign,
                        replaceVariable);
                }


                InstructionsUtils.DeleteInstructions(intermediateCode, new HashSet<int>() { i + 1 });
                Result = true;
            }
        }
    }
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
                if (firstInstruction.Kind != OperationKind.Assignment)
                    continue;
                var secondInstruction = localOperations[i + 1];
                if (secondInstruction.Kind != OperationKind.Assignment)
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