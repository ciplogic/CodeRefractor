﻿#region Uses

using System.Collections.Generic;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.Interpreters.Cil;

#endregion

namespace CodeRefractor.Analyze
{
    public static class InstructionsUtils
    {
        public static string ToHex(this int value)
        {
            return value.ToString("X");
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

        public static Dictionary<int, int> BuildLabelTable(LocalOperation[] operations, int[] labelOperations)
        {
            var labelTable = new Dictionary<int, int>();
            foreach (var i in labelOperations)
            {
                var operation = operations[i];
                var jumpTo = ((Label) operation).JumpTo;
                labelTable[jumpTo] = i;
            }
            return labelTable;
        }

        public static void DeleteInstructions(this CilMethodInterpreter intermediateCode,
            IEnumerable<int> instructionsToBeDeleted)
        {
            intermediateCode.MidRepresentation.DeleteInstructions(instructionsToBeDeleted);
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