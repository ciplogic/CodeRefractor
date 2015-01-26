#region Usings

using System.Collections.Generic;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.Licm
{
    internal static class LoopDetection
    {
        public static List<int> FindLoops(MetaMidRepresentation midRepresentation)
        {
            var useDef = midRepresentation.UseDef;
            var localOps = useDef.GetLocalOperations();

            var findStartLoopCandidates = FindStartLoopCandidates(localOps);
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
            var label = (Label) localOps[startPos + 1];
            var jumpTarget = label.JumpTo;

            var result = -1;
            for (var index = 0; index < localOps.Length; index++)
            {
                var op = localOps[index];
                if (op.Kind != OperationKind.BranchOperator)
                    continue;

                var branchOperator = (BranchOperator) op;
                if (branchOperator.JumpTo != jumpTarget) continue;
                result = index;
                break;
            }
            return result;
        }
    }
}