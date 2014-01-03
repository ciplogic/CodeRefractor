#region Usings

using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Util
{
    public static class InstructionsUtils
    {
        public static Assignment GetAssignment(this LocalOperation operation)
        {
            return operation.Value as Assignment;
        }

        public static bool Contains<T>(this T[] items, T value)
        {
            foreach (var item in items)
            {
                if (Equals(items, value))
                    return true;
            }
            return false;
        }

        public static Dictionary<int, int> BuildLabelTable(List<LocalOperation> operations)
        {
            var labelTable = new Dictionary<int, int>();
            labelTable.Clear();
            for (var i = 0; i < operations.Count; i++)
            {
                var operation = operations[i];
                switch (operation.Kind)
                {
                    case OperationKind.Label:
                        var jumpTo = (int) operation.Value;
                        labelTable[jumpTo] = i;
                        break;
                }
            }
            return labelTable;
        }

        public static void DeleteInstructions(this MetaMidRepresentation intermediateCode,
                                              IEnumerable<int> instructionsToBeDeleted)
        {
            var pos = 0;
            var liveOperations = new List<LocalOperation>();
            var toDelete = new HashSet<int>(instructionsToBeDeleted);
            foreach (var op in intermediateCode.LocalOperations)
            {
                if (!toDelete.Contains(pos))
                    liveOperations.Add(op);
                pos++;
            }
            intermediateCode.LocalOperations = liveOperations;
        }

    }
}