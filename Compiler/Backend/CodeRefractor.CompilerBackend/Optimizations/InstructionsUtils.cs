#region Usings

using System.Collections.Generic;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations
{
    public static class InstructionsUtils
    {
        public static Assignment GetAssignment(this LocalOperation operation)
        {
            return operation.Value as Assignment;
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
                                              HashSet<int> instructionsToBeDeleted)
        {
            var pos = 0;
            var liveOperations = new List<LocalOperation>();
            foreach (var op in intermediateCode.LocalOperations)
            {
                if (!instructionsToBeDeleted.Contains(pos))
                    liveOperations.Add(op);
                pos++;
            }
            intermediateCode.LocalOperations = liveOperations;
            instructionsToBeDeleted.Clear();
        }

    }
}