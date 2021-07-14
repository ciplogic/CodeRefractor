#region Uses

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;
using CodeRefractor.FrontEnd.SimpleOperations.Operators;
using CodeRefractor.MiddleEnd.Interpreters.Cil;

#endregion

namespace CodeRefractor.MiddleEnd.Optimizations.RedundantExpressions
{
    internal static class PrecomputeRepeatedUtils
    {
        public static LocalOperation CreateAssignLocalOperation(LocalVariable assignedTo, LocalVariable cacheVariable)
        {
            var assignment = new Assignment
            {
                AssignedTo = assignedTo,
                Right = cacheVariable
            };
            var localOperation = assignment;
            return localOperation;
        }

        public static CallMethodStatic GetMethodData(List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetMethodData(localOperations, index);
        }

        public static CallMethodStatic GetMethodData(List<LocalOperation> localOperations, int index)
        {
            return (CallMethodStatic) localOperations[index];
        }

        public static LocalVariable CreateCacheVariable(this CilMethodInterpreter interpreter,
            TypeDescription computedType)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var max = midRepresentation.Vars.VirtRegs.Max(vreg => vreg.Id) + 1;
            var cacheVariable = new LocalVariable
            {
                FixedType = computedType,
                Id = max,
                Kind = VariableKind.Vreg
            };
            cacheVariable.AutoName();
            midRepresentation.Vars.VirtRegs.Add(cacheVariable);
            return cacheVariable;
        }

        public static BinaryOperator GetBinaryOperator(this LocalOperation[] localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetBinaryOperator(localOperations, index);
        }

        public static BinaryOperator GetBinaryOperator(this LocalOperation[] localOperations, int index)
        {
            var localOperation = localOperations[index];
            return GetBinaryOperator(localOperation);
        }

        public static BinaryOperator GetBinaryOperator(this LocalOperation localOperation)
        {
            return (BinaryOperator) localOperation;
        }

        public static GetField GetFieldOperation(this IList<LocalOperation> localOperations, int[] calls, int i)
        {
            var index = calls[i];
            return GetFieldOperation(localOperations, index);
        }

        public static GetField GetFieldOperation(this IList<LocalOperation> localOperations, int index)
        {
            return (GetField) localOperations[index];
        }

        public static BranchOperator GetBranchOperator(this List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetBranchOperator(localOperations, index);
        }

        public static BranchOperator GetBranchOperator(this List<LocalOperation> localOperations, int index)
        {
            return (BranchOperator) localOperations[index];
        }

        public static UnaryOperator GetUnaryOperator(this List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetUnaryOperator(localOperations, index);
        }

        public static UnaryOperator GetUnaryOperator(this List<LocalOperation> localOperations, int index)
        {
            return (UnaryOperator) localOperations[index];
        }
    }
}