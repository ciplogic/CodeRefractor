using System.Collections.Generic;
using System.IO;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.CompilerBackend.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments
{
    internal class RemoveDeadStoresInBlockOptimizationPass : BlockOptimizationPass
    {
        private readonly Dictionary<LocalVariable, int> _dictionary =
            new Dictionary<LocalVariable, int>();

        public override bool OptimizeBlock(List<LocalOperation> localOperations, int startRange, int endRange)
        {
            _dictionary.Clear();
            for (var i = startRange; i <= endRange; i++)
            {
                var op = localOperations[i];

                var definition = op.GetUseDefinition();
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
            bool canApply = false;
            switch (kind)
            {
                case OperationKind.Assignment:
                case OperationKind.BinaryOperator:
                case OperationKind.BranchOperator:
                case OperationKind.UnaryOperator:
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