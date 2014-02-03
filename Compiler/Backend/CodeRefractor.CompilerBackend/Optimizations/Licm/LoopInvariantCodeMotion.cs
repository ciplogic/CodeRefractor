using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.Licm
{
    class LoopInvariantCodeMotion : ResultingGlobalOptimizationPass
    {
        public override bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            var loopStarts = LoopDetection.FindLoops(midRepresentation.MidRepresentation);
            return loopStarts.Count != 0;
        }

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var loopStarts = LoopDetection.FindLoops(methodInterpreter.MidRepresentation);
            foreach (var loopStart in loopStarts)
            {
                var loopEnd = LoopDetection.GetEndLoop(methodInterpreter.MidRepresentation.LocalOperations, loopStart);
                var allDefinedVariables = GetAllDefinedVariables(methodInterpreter.MidRepresentation, loopStart, loopEnd);
                var allInvariantInstructions = GetAllInvariantInstructions(methodInterpreter.MidRepresentation, loopStart, loopEnd,
                                                                           allDefinedVariables);
                if (allInvariantInstructions.Count == 0)
                    continue;
                PerformMoveInstructions(methodInterpreter.MidRepresentation, loopStart, allInvariantInstructions);
                    methodInterpreter.MidRepresentation.UpdateUseDef();
                Result = true;
                return;
            }
        }

        private static void PerformMoveInstructions(MetaMidRepresentation intermediateCode, int loopStart, List<int> allInvariantInstructions)
        {
            var localOps = intermediateCode.LocalOperations;

            var licmBlock = new List<LocalOperation>();
            for (var index = allInvariantInstructions.Count - 1; index >= 0; index--)
            {
                var invariantInstruction = allInvariantInstructions[index];
                licmBlock.Add(localOps[invariantInstruction]);
                localOps.RemoveAt(invariantInstruction);
            }
            for (int index = 0; index < licmBlock.Count; index++)
            {
                var operation = licmBlock[index];
                localOps.Insert(loopStart + index, operation);
            }
        }

        private static List<int> GetAllInvariantInstructions(MetaMidRepresentation intermediateCode, int loopStart, int loopEnd, HashSet<LocalVariable> getAllDefinedVariables)
        {
            var localOps = intermediateCode.LocalOperations;


            var result = new List<int>();
            var useDef = intermediateCode.UseDef;
            for (var index = loopStart; index <= loopEnd; index++)
            {
                var op = localOps[index];
                switch (op.Kind)
                {
                    default:continue;
                    case OperationKind.UnaryOperator:
                    case OperationKind.Call:
                    case OperationKind.BinaryOperator:
                    case OperationKind.GetField:
                    case OperationKind.Assignment:
                        break;
                }
                if(op.Kind==OperationKind.Call)
                {
                    var methodData =EvaluatePureFunctionWithConstantCall.ComputeAndEvaluatePurityOfCall(op);
                    if(!methodData.IsPure)
                        continue;
                }
                var usages = useDef.GetUsages(index);
                var isInvariant = true;
                foreach (var usage in usages)
                {
                    if (!getAllDefinedVariables.Contains(usage)) continue;
                    isInvariant = false;
                    break;
                }
                if(!isInvariant)
                    continue;
                result.Add(index);
            }

            return result;

        }

        private static HashSet<LocalVariable> GetAllDefinedVariables(MetaMidRepresentation intermediateCode, int loopStart, int loopEnd)
        {
            var localOps = intermediateCode.LocalOperations;
            var result = new HashSet<LocalVariable>();
            for (var index = loopStart; index <= loopEnd; index++)
            {
                var op = localOps[index];
                var definition = op.GetDefinition();
                if(definition==null)
                    continue;
                result.Add(definition);
            }

            return result;
        }
    }
}
