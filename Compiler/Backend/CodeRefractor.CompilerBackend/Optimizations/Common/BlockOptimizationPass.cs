#region Usings

using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    internal abstract class BlockOptimizationPass : ResultingInFunctionOptimizationPass
    {
        public LocalOperation[] GetInstructionRange(
            LocalOperation[] operations, int startInstruction, int endInstruction, bool cleanInstructions = true)
        {
            var result = new List<LocalOperation>();
            for (var i = startInstruction; i <= endInstruction; i++)
            {
                var op = operations[i];
                if (cleanInstructions)
                {
                    if (op.Kind == OperationKind.Comment)
                        continue;
                }
                result.Add(op);
            }
            return result.ToArray();
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            var sortedLabelPos = BuildBlockOperations(interpreter);
            var startPos = 0;
            var result = false;
            foreach (var labelPos in sortedLabelPos)
            {
                result |= TryOptimizeBlock(interpreter, startPos, labelPos - 1, localOperations);
                if (result)
                {
                    Result = true;
                    return;
                }
                startPos = labelPos + 1;
            }
            Result = TryOptimizeBlock(interpreter, startPos, localOperations.Length - 1, localOperations);
        }

        private static List<int> BuildBlockOperations(MethodInterpreter methodInterpreter)
        {
            var useDef = methodInterpreter.MidRepresentation.UseDef;
            var result = new List<int>();
            result.AddRange(useDef.GetOperations(OperationKind.Label));
            result.AddRange(useDef.GetOperations(OperationKind.BranchOperator));
            result.AddRange(useDef.GetOperations(OperationKind.AlwaysBranch));
            result.Sort();
            return result;
        }

        private bool TryOptimizeBlock(MethodInterpreter localOperations, int startRange, int endRange,
            LocalOperation[] operations)
        {
            if (startRange >= endRange)
                return false;
            return OptimizeBlock(localOperations, startRange, endRange, operations);
        }

        public abstract bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange,
            LocalOperation[] operations);
    }
}