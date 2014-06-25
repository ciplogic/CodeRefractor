﻿#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Purity;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.Licm
{
    [Optimization(Category = OptimizationCategories.CommonSubexpressionsElimination)]
    internal class LoopInvariantCodeMotion : ResultingGlobalOptimizationPass
    {
        public override bool CheckPreconditions(MethodInterpreter midRepresentation)
        {
            var loopStarts = LoopDetection.FindLoops(midRepresentation.MidRepresentation);
            return loopStarts.Count != 0;
        }

        public override void OptimizeOperations(MethodInterpreter interpreter)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var useDef = midRepresentation.UseDef;
            var loopStarts = LoopDetection.FindLoops(midRepresentation);
            foreach (var loopStart in loopStarts)
            {
                var loopEnd = LoopDetection.GetEndLoop(useDef.GetLocalOperations(), loopStart);
                var allDefinedVariables = GetAllDefinedVariables(midRepresentation, loopStart, loopEnd);
                var allInvariantInstructions = GetAllInvariantInstructions(midRepresentation, loopStart, loopEnd,
                    allDefinedVariables);
                if (allInvariantInstructions.Count == 0)
                    continue;
                PerformMoveInstructions(midRepresentation, loopStart, allInvariantInstructions);
                Result = true;
                return;
            }
        }

        private static void PerformMoveInstructions(MetaMidRepresentation intermediateCode, int loopStart,
            List<int> allInvariantInstructions)
        {
            var localOps = intermediateCode.LocalOperations;

            var licmBlock = new List<LocalOperation>();
            for (var index = allInvariantInstructions.Count - 1; index >= 0; index--)
            {
                var invariantInstruction = allInvariantInstructions[index];
                licmBlock.Add(localOps[invariantInstruction]);
                localOps.RemoveAt(invariantInstruction);
            }
            for (var index = 0; index < licmBlock.Count; index++)
            {
                var operation = licmBlock[index];
                localOps.Insert(loopStart + index, operation);
            }
        }

        private static List<int> GetAllInvariantInstructions(MetaMidRepresentation intermediateCode, int loopStart,
            int loopEnd, HashSet<LocalVariable> getAllDefinedVariables)
        {
            var useDef = intermediateCode.UseDef;
            var localOps = useDef.GetLocalOperations();


            var result = new List<int>();
            for (var index = loopStart; index <= loopEnd; index++)
            {
                var op = localOps[index];
                switch (op.Kind)
                {
                    default:
                        continue;
                    case OperationKind.UnaryOperator:
                    case OperationKind.Call:
                    case OperationKind.BinaryOperator:
                    case OperationKind.GetField:
                    case OperationKind.Assignment:
                        break;
                }
                if (op.Kind == OperationKind.Call)
                {
                    var methodData = EvaluatePureFunctionWithConstantCall.ComputeAndEvaluatePurityOfCall(op);
                    if (!methodData.Interpreter.AnalyzeProperties.IsPure)
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
                if (!isInvariant)
                    continue;
                result.Add(index);
            }

            return result;
        }

        private static HashSet<LocalVariable> GetAllDefinedVariables(MetaMidRepresentation intermediateCode,
            int loopStart, int loopEnd)
        {
            var localOps = intermediateCode.UseDef.GetLocalOperations();
            var result = new HashSet<LocalVariable>();
            for (var index = loopStart; index <= loopEnd; index++)
            {
                var op = localOps[index];
                var definition = op.GetDefinition();
                if (definition == null)
                    continue;
                result.Add(definition);
            }

            return result;
        }
    }
}