#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.ReachabilityDfa
{
    public class DeleteJumpNextLine : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var operations = methodInterpreter.MidRepresentation.LocalOperations.ToArray();
            var labelTable = methodInterpreter.GetLabelTable();
            var toRemove = new List<int>();
            foreach (var labelInfo in labelTable)
            {
                var i = labelInfo.Value-1;
                if(i<0)
                    continue;
                var operation = operations[i];
                switch (operation.Kind)
                {
                    case OperationKind.AlwaysBranch:
                        var jumpLabel = labelTable[(int)operation.Value];

                        if (jumpLabel != labelInfo.Value)
                            continue;
                        toRemove.Add(i);
                        continue;
                    case OperationKind.BranchOperator:

                        var destAssignment = (BranchOperator)operation.Value;
                        var jumpTo = labelTable[destAssignment.JumpTo];
                        if (jumpTo != labelInfo.Value)
                            continue;

                        toRemove.Add(i);
                        continue;
                    default:
                        continue;
                }
            }
            if(toRemove.Count==0)
                return;
            methodInterpreter.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}