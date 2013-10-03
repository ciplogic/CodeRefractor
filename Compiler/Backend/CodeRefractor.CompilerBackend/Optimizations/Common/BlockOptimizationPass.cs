using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    abstract class BlockOptimizationPass : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var localOperations = intermediateCode.LocalOperations;
            var sortedLabelPos = BuildLabelTable(localOperations);
            var startPos = 0;
            var result = false;
            foreach (var labelPos in sortedLabelPos)
            {
                result |= TryOptimizeBlock(intermediateCode, startPos, labelPos - 1);
                if(result)
                {
                    Result = true;
                    return;
                }
                startPos = labelPos + 1;
            }
            TryOptimizeBlock(intermediateCode, startPos, localOperations.Count - 1);
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

        bool TryOptimizeBlock(MetaMidRepresentation localOperations, int startRange, int endRange)
        {
            if (startRange == endRange)
                return false;
            return OptimizeBlock(localOperations, startRange, endRange);
        }
        public abstract bool OptimizeBlock(MetaMidRepresentation localOperations, int startRange, int endRange);
    }
}