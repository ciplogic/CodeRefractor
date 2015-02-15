using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Mono.Reflection;

namespace CodeRefractor.FrontEnd
{
    static class ExceptionCatchClauseRanges
    {
        private static List<KeyValuePair<int, int>> GetExceptionRanges(List<ExceptionHandlingClause> handlingExceptions)
        {
            var result = new List<KeyValuePair<int, int>>();
            foreach (var handlingClause in handlingExceptions)
            {
                if (handlingClause.Flags != ExceptionHandlingClauseOptions.Clause || handlingClause.CatchType == null)
                    continue;
                result.Add(new KeyValuePair<int, int>(handlingClause.HandlerOffset,
                    handlingClause.HandlerOffset+handlingClause.HandlerLength));
            }
            return result;
        }

        internal static List<KeyValuePair<int, int>> ComputeExceptionInstructionRanges(MethodBase method)
        {
            var exceptionHandlers = method.GetMethodBody().ExceptionHandlingClauses.ToList();
            return GetExceptionRanges(exceptionHandlers);
        }

        //validates if the instruction's offset is inside the ranges that are defiend as start and length
        internal static bool IndexInRanges(Instruction instruction, List<KeyValuePair<int, int>> ranges)
        {
            var index = instruction.Offset;
            return ranges.Any(range => (index >= range.Key)  && (index < range.Value));
        }
    }
}