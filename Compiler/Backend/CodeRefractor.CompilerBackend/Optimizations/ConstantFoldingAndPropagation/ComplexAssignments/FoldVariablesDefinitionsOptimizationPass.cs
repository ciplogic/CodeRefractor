using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class FoldVariablesDefinitionsOptimizationPass : ResultingInFunctionOptimizationPass
    {
        readonly Dictionary<LocalVariable, int> _definitionsDictionary = new Dictionary<LocalVariable, int>();

        readonly Dictionary<LocalVariable, int> _usagesDictionary = new Dictionary<LocalVariable, int>();

        public override void OptimizeOperations(MethodInterpreter methodInterpreter)
        {
            var metaMidRepresentation = methodInterpreter.MidRepresentation;
            var localOperations = metaMidRepresentation.LocalOperations;
            _definitionsDictionary.Clear();
            BuildDefinitionDictionary(localOperations, metaMidRepresentation.UseDef);
            RemoveNonUniqueDefinitions(_definitionsDictionary);
            RemoveNonUniqueDefinitions(_usagesDictionary);
            var toPatch = new List<int>();
            foreach (var targetOp in localOperations.Where(op=>op.Kind==OperationKind.Assignment))
            {
                var assignment = targetOp.GetAssignment();
                var rightVar = assignment.Right as LocalVariable;
                if(rightVar==null)
                    continue;
                if(!_usagesDictionary.ContainsKey(rightVar))
                    continue;
                var leftVar = assignment.AssignedTo;
                if (!_definitionsDictionary.ContainsKey(rightVar)
                    || !_definitionsDictionary.ContainsKey(leftVar))
                    continue;
                var rightId = _definitionsDictionary[rightVar];
                var leftId = _definitionsDictionary[leftVar];
                if (leftId- rightId != 1)
                    continue;
                toPatch.Add(leftId);
            }
            if(toPatch.Count==0)
                return;
            var patchArr = toPatch.ToArray();
            Array.Sort(patchArr);
            patchArr= patchArr.Reverse().ToArray();
            PatchInstructions(localOperations, patchArr);
        }

        private void PatchInstructions(List<LocalOperation> localOperations, IEnumerable<int> toPatch)
        {
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
                localOperations.RemoveAt(line);
            }
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

        private void BuildDefinitionDictionary(List<LocalOperation> localOperations, UseDefDescription useDef)
        {
            for (var i = 0; i < localOperations.Count; i++)
            {
                var def = useDef.GetDefinition(i);
                UpdateDefinitionDictionaryForIndex(i, def);
                var usages = useDef.GetUsages(i); //op.GetUsages();
                UpdateUsagesDictionaryForIndex(i, usages);

            }
        }

        private void UpdateUsagesDictionaryForIndex(int i, LocalVariable[] usages)
        {
            foreach (var localVariable in usages)
            {
                if (_usagesDictionary.ContainsKey(localVariable))
                    _usagesDictionary[localVariable] = -1;
                else
                {
                    _usagesDictionary[localVariable] = i;
                }
            }
        }

        private void UpdateDefinitionDictionaryForIndex(int i, LocalVariable def)
        {
            if (def == null)
                return;

            if (_definitionsDictionary.ContainsKey(def))
                _definitionsDictionary[def] = -1;
            else
            {
                _definitionsDictionary[def] = i;
            }
        }
    }
}