#region Usings

using System.Collections.Generic;
using System.IO;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    internal class RemoveDeadStoresInBlockOptimizationPass : BlockOptimizationPass
    {
        public override bool OptimizeBlock(MethodInterpreter midRepresentation, int startRange, int endRange,
            LocalOperation[] operations)
        {
            var dictionary = new Dictionary<LocalVariable, int>();
            dictionary.Clear();
            var useDef = midRepresentation.MidRepresentation.UseDef;
            for (var i = startRange; i <= endRange; i++)
            {
                var localOperations = midRepresentation.MidRepresentation.LocalOperations;
                var op = localOperations[i];

                var definition = op.GetDefinition();
                if (definition != null)
                {
                    if (!dictionary.ContainsKey(definition))
                        dictionary[definition] = i;
                    else
                    {
                        if (TryRemoveLine(dictionary[definition], localOperations))
                            return true;
                    }
                }
                var usages = useDef.GetUsages(i);
                foreach (var usage in usages)
                {
                    dictionary.Remove(usage);
                }
            }
            return false;
        }

        private static bool TryRemoveLine(int i, List<LocalOperation> localOperations)
        {
            var kind = localOperations[i].Kind;
            switch (kind)
            {
                case OperationKind.Assignment:
                case OperationKind.BinaryOperator:
                case OperationKind.BranchOperator:
                case OperationKind.UnaryOperator:
                case OperationKind.GetField:
                    break;

                case OperationKind.Call:
                    return false;
                default:
                    throw new InvalidDataException("Operation should be handled");
            }
            localOperations.RemoveAt(i);
            return true;
        }
    }
}