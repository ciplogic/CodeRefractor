using System.Collections.Generic;
using System.IO;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    internal class RemoveDeadStoresInBlockOptimizationPass : BlockOptimizationPass
    {
        private readonly Dictionary<LocalVariable, int> _dictionary =
            new Dictionary<LocalVariable, int>();

        public override bool OptimizeBlock(MetaMidRepresentation midRepresentation, int startRange, int endRange)
        {
            _dictionary.Clear();
            for (var i = startRange; i <= endRange; i++)
            {
                var localOperations = midRepresentation.LocalOperations;
                var op = localOperations[i];

                var definition = op.GetDefinition();
                if(definition!=null)
                {
                    if (!_dictionary.ContainsKey(definition))
                        _dictionary[definition] = i;
                    else
                    {
                        if (TryRemoveLine(_dictionary[definition], localOperations))
                            return true;
                    }
                }
                var usages = op.GetUsages();
                foreach (var usage in usages)
                {
                    _dictionary.Remove(usage);
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