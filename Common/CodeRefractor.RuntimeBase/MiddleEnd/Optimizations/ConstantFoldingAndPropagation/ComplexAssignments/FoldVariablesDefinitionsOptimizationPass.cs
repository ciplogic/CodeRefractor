#region Uses

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.UseDefs;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    class FoldVariablesDefinitionsOptimizationPass : OptimizationPassBase
    {
        public FoldVariablesDefinitionsOptimizationPass()
            : base(OptimizationKind.InFunction)
        {
        }

        public override bool ApplyOptimization(CilMethodInterpreter interpreter, ClosureEntities closure)
        {
            var definitionsDictionary = new Dictionary<LocalVariable, int>();

            var usagesDictionary = new Dictionary<LocalVariable, int>();

            var metaMidRepresentation = interpreter.MidRepresentation;
            var localOperations = interpreter.MidRepresentation.UseDef.GetLocalOperations();
            definitionsDictionary.Clear();
            BuildDefinitionDictionary(localOperations, metaMidRepresentation.UseDef, definitionsDictionary,
                usagesDictionary);
            RemoveNonUniqueDefinitions(definitionsDictionary);
            RemoveNonUniqueDefinitions(usagesDictionary);
            var toPatch = new List<int>();
            foreach (var targetOp in localOperations.Where(op => op.Kind == OperationKind.Assignment))
            {
                var assignment = targetOp.Get<Assignment>();
                var rightVar = assignment.Right as LocalVariable;
                if (rightVar == null)
                    continue;
                if (!usagesDictionary.ContainsKey(rightVar))
                    continue;
                var leftVar = assignment.AssignedTo;
                if (!definitionsDictionary.ContainsKey(rightVar)
                    || !definitionsDictionary.ContainsKey(leftVar))
                    continue;
                var rightId = definitionsDictionary[rightVar];
                var leftId = definitionsDictionary[leftVar];
                if (leftId - rightId != 1)
                    continue;
                toPatch.Add(leftId);
            }
            if (toPatch.Count == 0)
                return false;
            var toRemove = PatchInstructions(localOperations, toPatch);
            interpreter.DeleteInstructions(toRemove);
            return true;
        }

        List<int> PatchInstructions(LocalOperation[] localOperations, IEnumerable<int> toPatch)
        {
            var toRemove = new List<int>();
            foreach (var line in toPatch)
            {
                var assignment = localOperations[line].Get<Assignment>();
                var destOperation = localOperations[line - 1];
                switch (destOperation.Kind)
                {
                    case OperationKind.Assignment:
                        var operatorAssig = (Assignment)destOperation;
                        operatorAssig.AssignedTo = assignment.AssignedTo;
                        break;
                    case OperationKind.UnaryOperator:
                    case OperationKind.BinaryOperator:
                        var operatorData = (OperatorBase)destOperation;
                        operatorData.AssignedTo = assignment.AssignedTo;
                        break;
                    default:
                        continue;
                }
                toRemove.Add(line);
            }
            return toRemove;
        }

        static void RemoveNonUniqueDefinitions(Dictionary<LocalVariable, int> dictionaryPositions)
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

        void BuildDefinitionDictionary(LocalOperation[] localOperations, UseDefDescription useDef,
            Dictionary<LocalVariable, int> definitionsDictionary, Dictionary<LocalVariable, int> usagesDictionary)
        {
            for (var i = 0; i < localOperations.Length; i++)
            {
                var def = useDef.GetDefinition(i);
                UpdateDefinitionDictionaryForIndex(i, def, definitionsDictionary);
                var usages = useDef.GetUsages(i);
                UpdateUsagesDictionaryForIndex(i, usages, usagesDictionary);
            }
        }

        void UpdateUsagesDictionaryForIndex(int i, LocalVariable[] usages,
            Dictionary<LocalVariable, int> usagesDictionary)
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

        void UpdateDefinitionDictionaryForIndex(int i, LocalVariable def,
            Dictionary<LocalVariable, int> definitionsDictionary)
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