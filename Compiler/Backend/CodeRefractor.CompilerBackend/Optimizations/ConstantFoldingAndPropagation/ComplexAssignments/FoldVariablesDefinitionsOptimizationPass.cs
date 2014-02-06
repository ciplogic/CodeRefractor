using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class FoldVariablesDefinitionsOptimizationPass : ResultingInFunctionOptimizationPass
    {
        
        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var definitionsDictionary = new Dictionary<LocalVariable, int>();

            var usagesDictionary = new Dictionary<LocalVariable, int>();

            var metaMidRepresentation = methodInterpreter.MidRepresentation;
            var localOperations = metaMidRepresentation.LocalOperations.ToArray();
            definitionsDictionary.Clear();
            BuildDefinitionDictionary(localOperations, metaMidRepresentation.UseDef, definitionsDictionary, usagesDictionary);
            RemoveNonUniqueDefinitions(definitionsDictionary);
            RemoveNonUniqueDefinitions(usagesDictionary);
            var toPatch = new List<int>();
            foreach (var targetOp in localOperations.Where(op=>op.Kind==OperationKind.Assignment))
            {
                var assignment = targetOp.GetAssignment();
                var rightVar = assignment.Right as LocalVariable;
                if(rightVar==null)
                    continue;
                if(!usagesDictionary.ContainsKey(rightVar))
                    continue;
                var leftVar = assignment.AssignedTo;
                if (!definitionsDictionary.ContainsKey(rightVar)
                    || !definitionsDictionary.ContainsKey(leftVar))
                    continue;
                var rightId = definitionsDictionary[rightVar];
                var leftId = definitionsDictionary[leftVar];
                if (leftId- rightId != 1)
                    continue;
                toPatch.Add(leftId);
            }
            if(toPatch.Count==0)
                return;
            var toRemove = PatchInstructions(localOperations, toPatch);
            methodInterpreter.DeleteInstructions(toRemove);
        }

        private List<int> PatchInstructions(LocalOperation[] localOperations, IEnumerable<int> toPatch)
        {
            var toRemove = new List<int>();
            foreach (var line in toPatch)
            {
                var assignment = localOperations[line].GetAssignment();
                var destOperation = localOperations[line-1];
                switch (destOperation.Kind)
                {
                    case OperationKind.Assignment:
                        var operatorAssig = (Assignment) destOperation.Value;
                        operatorAssig.AssignedTo = assignment.AssignedTo;
                        Result = true;

                        break;
                    case OperationKind.UnaryOperator:
                    case OperationKind.BinaryOperator:
                        var operatorData = (OperatorBase) destOperation.Value;
                        operatorData.AssignedTo = assignment.AssignedTo;
                        Result = true;
                        break;
                    default:
                        continue;
                }
                toRemove.Add(line);
            }
            return toRemove;
        }

        private static void RemoveNonUniqueDefinitions(Dictionary<LocalVariable, int> dictionaryPositions)
        {
            var toRemove = new HashSet<LocalVariable>();
            
            foreach (var item in dictionaryPositions)
            {
                if (item.Value == -1)
                    toRemove.Add(item.Key);
            }
            foreach (var variable in toRemove)
            {
                dictionaryPositions.Remove(variable);
            }
        }

        private void BuildDefinitionDictionary(LocalOperation[] localOperations, UseDefDescription useDef, Dictionary<LocalVariable, int> definitionsDictionary, Dictionary<LocalVariable, int> usagesDictionary)
        {
            for (var i = 0; i < localOperations.Length; i++)
            {
                var def = useDef.GetDefinition(i);
                UpdateDefinitionDictionaryForIndex(i, def, definitionsDictionary);
                var usages = useDef.GetUsages(i);
                UpdateUsagesDictionaryForIndex(i, usages, usagesDictionary);

            }
        }

        private void UpdateUsagesDictionaryForIndex(int i, LocalVariable[] usages, Dictionary<LocalVariable, int> usagesDictionary)
        {
            foreach (var localVariable in usages)
            {
                if (usagesDictionary.ContainsKey(localVariable))
                    usagesDictionary[localVariable] = -1;
                else
                {
                    usagesDictionary[localVariable] = i;
                }
            }
        }

        private void UpdateDefinitionDictionaryForIndex(int i, LocalVariable def, Dictionary<LocalVariable, int> definitionsDictionary)
        {
            if (def == null)
                return;

            if (definitionsDictionary.ContainsKey(def))
                definitionsDictionary[def] = -1;
            else
            {
                definitionsDictionary[def] = i;
            }
        }
    }
}