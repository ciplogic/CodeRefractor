using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.CompilerBackend.Optimizations.Licm
{
    static class LoopDetection
    {
        public static List<int> FindLoops(MetaMidRepresentation midRepresentation)
        {
            var useDef = midRepresentation.UseDef;
            var localOps = useDef.GetLocalOperations();

            var findStartLoopCandidates = FindStartLoopCandidates(localOps.ToArray());
            var result = new List<int>();
            if (findStartLoopCandidates.Count == 0)
                return result;
            foreach (var findStartLoopCandidate in findStartLoopCandidates)
            {
                var endLoop = GetEndLoop(localOps, findStartLoopCandidate);
                if (endLoop != -1)
                    result.Add(findStartLoopCandidate);
            }

            return result;
        }

        private static List<int> FindStartLoopCandidates(LocalOperation[] localOps)
        {
            var findStartLoopCandidates = new List<int>();
            for (var index = 0; index < localOps.Length; index++)
            {
                var op = localOps[index];
                if (op.Kind != OperationKind.AlwaysBranch)
                    continue;
                if (localOps[index + 1].Kind != OperationKind.Label) continue;
                findStartLoopCandidates.Add(index);
            }
            return findStartLoopCandidates;
        }

        public static int GetEndLoop(LocalOperation[] localOps, int startPos)
        {
            var jumpTarget = (int)localOps[startPos + 1].Value;

            var result = -1;
            for (var index = 0; index < localOps.Length; index++)
            {
                var op = localOps[index];
                if (op.Kind != OperationKind.BranchOperator)
                    continue;

                var branchOperator = (BranchOperator)op.Value;
                if (branchOperator.JumpTo != jumpTarget) continue;
                result = index;
                break;
            }
            return result;
        }
    }
}