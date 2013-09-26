using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
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
                result |= OptimizeBlock(localOperations, startPos, labelPos - 1);
                if(result)
                {
                    Result = true;
                    return;
                }
                startPos = labelPos + 1;
            }
            OptimizeBlock(localOperations, startPos, localOperations.Count - 1);
            result |= Result;
            Result = result;
        }

        private static List<int> BuildLabelTable(List<LocalOperation> localOperations)
        {
            var labels = InstructionsUtils.BuildLabelTable(localOperations);
            var sortedLabelPos = new SortedSet<int>(labels.Values).ToList();
            return sortedLabelPos;
        }

        public abstract bool OptimizeBlock(List<LocalOperation> localOperations, int startRange, int endRange);
    }
}