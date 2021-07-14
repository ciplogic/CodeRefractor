#region Uses

using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Optimizations;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Jumps
{
    [Optimization(Category = OptimizationCategories.UseDef)]
    public class MergeConsecutiveLabels : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(CilMethodInterpreter interpreter)
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
                var jumpId = ((Label) operation).JumpTo;
                var jumpId2 = ((Label) operation2).JumpTo;
                ;
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
                        var jumpTo = ((Label)operation).JumpTo;
                        if (jumpId2 == jumpTo)
                            ((Label)operation).JumpTo = jumpId;
                        break;
                    case OperationKind.BranchOperator:
                        var destAssignment = (BranchOperator)operation;
                        if (destAssignment.JumpTo == jumpId2)
                            destAssignment.JumpTo = jumpId;
                        break;
                }
            }
        }
    }
}