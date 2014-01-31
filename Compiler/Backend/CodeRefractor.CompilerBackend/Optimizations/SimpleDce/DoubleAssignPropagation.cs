#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.Analyze;
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
    internal class DoubleAssignPropagation : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.LocalOperations;
            var toPatch = Analyze(localOperations.ToArray());
            if(toPatch.Count==0)
                return;
            ApplyOptimization(methodInterpreter, toPatch, localOperations);
        }

        private void ApplyOptimization(MethodInterpreter methodInterpreter, List<int> toPatch, List<LocalOperation> localOperations)
        {
            foreach (var patchLine in toPatch)
            {
                var prevOp = localOperations[patchLine - 1];
                var assign = localOperations[patchLine].GetAssignment();
                prevOp.SwitchUsageWithDefinition(assign.Right as LocalVariable, assign.AssignedTo);
            }
            methodInterpreter.DeleteInstructions(toPatch);
            Result = true;
        }

        private static List<int> Analyze(LocalOperation[] localOperations)
        {
            var count = localOperations.Length;
            var toPatch = new List<int>();
            for (var i = 1; i < count; i++)
            {
                var targetAssignOp = localOperations[i];
                if (targetAssignOp.Kind != OperationKind.Assignment)
                    continue;
                var assign = targetAssignOp.GetAssignment();
                var rightVar = assign.Right as LocalVariable;
                if (rightVar == null)
                    continue;
                var prevOp = localOperations[i - 1];

                if (prevOp.Kind != OperationKind.BinaryOperator)
                    continue;
                var getDefinedVar = prevOp.GetDefinition();
                if (getDefinedVar == null)
                    continue;
                if (!getDefinedVar.Equals(rightVar))
                    continue;
                var usagesOfVar = localOperations.GetVariableUsages(rightVar);
                if (usagesOfVar.Count != 1)
                    continue;
                toPatch.Add(i);
            }
            return toPatch;
        }
    }
}