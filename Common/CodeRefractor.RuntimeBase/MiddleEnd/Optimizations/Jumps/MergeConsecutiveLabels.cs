#region Usings

using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Jumps
{
    internal class MergeConsecutiveLabels : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var useDef = interpreter.MidRepresentation.UseDef;
            var operations = useDef.GetLocalOperations();

            var labelIndices = useDef.GetOperationsOfKind(OperationKind.Label);
            var found = labelIndices.Length == 0;
            if (!found)
                return;
            foreach (var i in labelIndices)
            {
                var operation2 = operations[i + 1];
                if (operation2.Kind != OperationKind.Label)
                    continue;

                var operation = operations[i];
                var jumpId = (int) operation.Value;
                var jumpId2 = (int) operation2.Value;
                OptimizeConsecutiveLabels(operations, jumpId, jumpId2);
                interpreter.DeleteInstructions(new[] {i + 1});
                Result = true;
            }
        }

        private static void OptimizeConsecutiveLabels(LocalOperation[] operations, int jumpId, int jumpId2)
        {
            for (var i = 0; i < operations.Length - 2; i++)
            {
                var operation = operations[i];
                if (!operation.IsBranchOperation())
                    continue;
                switch (operation.Kind)
                {
                    case OperationKind.AlwaysBranch:
                        var jumpTo = (int) operation.Value;
                        if (jumpId2 == jumpTo)
                            operation.Value = jumpId;
                        break;
                    case OperationKind.BranchOperator:
                        var destAssignment = (BranchOperator) operation.Value;
                        if (destAssignment.JumpTo == jumpId2)
                            destAssignment.JumpTo = jumpId;
                        break;
                }
            }
        }
    }
}