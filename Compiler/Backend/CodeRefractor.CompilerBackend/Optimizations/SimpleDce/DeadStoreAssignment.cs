using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.CompilerBackend.Optimizations.SimpleDce
{
    class DeadStoreAssignment : ResultingInFunctionOptimizationPass
    {
        public override void OptimizeOperations(MetaMidRepresentation intermediateCode)
        {
            var pos = -1;
            var toRemove = new HashSet<int>();
            var localOperations = intermediateCode.LocalOperations;
            foreach (var op in localOperations)
            {
                pos++;

                var opKind = op.Kind;
                if (opKind != LocalOperation.Kinds.Assignment
                   && opKind != LocalOperation.Kinds.BinaryOperator
                    && opKind != LocalOperation.Kinds.UnaryOperator)
                    continue;
                var variableDefinition = op.GetUseDefinition();
                if(variableDefinition==null)
                    continue;

                var usagePos = intermediateCode.GetVariableUsages(variableDefinition);
                if (usagePos.Count == 0)
                {
                    toRemove.Add(pos);
                }
            }
            if(toRemove.Count==0)
                return;
            intermediateCode.DeleteInstructions(toRemove);
            Result = true;
        }
    }
}
