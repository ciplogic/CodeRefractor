using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    abstract class BlockOptimizationPass : ResultingInFunctionOptimizationPass
    {
        public List<LocalOperation> GetInstructionRange(
            MethodInterpreter intermediateCode, int startInstruction, int endInstruction, bool cleanInstructions = true)
        {
            var result = new List<LocalOperation>();
            var operations = intermediateCode.MidRepresentation.LocalOperations;
            for (var i = startInstruction; i <= endInstruction; i++)
            {
                var op = operations[i];
                if (cleanInstructions)
                {
                    if(op.Kind==OperationKind.Comment)
                        continue;
                }
                result.Add(op);
            }
            return result;
        }
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var localOperations = methodInterpreter.MidRepresentation.LocalOperations;
            var sortedLabelPos = BuildLabelTable(localOperations);
            var startPos = 0;
            var result = false;
            foreach (var labelPos in sortedLabelPos)
            {
                result |= TryOptimizeBlock(methodInterpreter, startPos, labelPos - 1);
                if(result)
                {
                    Result = true;
                    return;
                }
                startPos = labelPos + 1;
            }
            TryOptimizeBlock(methodInterpreter, startPos, localOperations.Count - 1);
            result |= Result;
            Result = result;
        }

        private static List<int> BuildLabelTable(List<LocalOperation> localOperations)
        {
            var labels = InstructionsUtils.BuildLabelTable(localOperations);
            
            var sortedLabelPos = new SortedSet<int>(labels.Values);
            for (var index = 0; index < localOperations.Count; index++)
            {
                var operation = localOperations[index];
                switch (operation.Kind)
                {
                    case OperationKind.BranchOperator:
                    case OperationKind.AlwaysBranch:
                        sortedLabelPos.Add(index);
                        break;
                }
            }

            return sortedLabelPos.ToList();
        }

        bool TryOptimizeBlock(MethodInterpreter localOperations, int startRange, int endRange)
        {
            if (startRange >= endRange)
                return false;
            return OptimizeBlock(localOperations, startRange, endRange);
        }
        public abstract bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange);
    }
}