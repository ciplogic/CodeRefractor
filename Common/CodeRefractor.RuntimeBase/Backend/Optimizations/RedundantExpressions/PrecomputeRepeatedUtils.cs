#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.RedundantExpressions
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
            var localOperation = new LocalOperation()
            {
                Kind = OperationKind.Assignment,
                Value = assignment
            };
            return localOperation;
        }


        public static MethodData GetMethodData(List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetMethodData(localOperations, index);
        }

        public static MethodData GetMethodData(List<LocalOperation> localOperations, int index)
        {
            return (MethodData) localOperations[index].Value;
        }

        public static LocalVariable CreateCacheVariable(this MethodInterpreter interpreter,
            TypeDescription computedType)
        {
            var midRepresentation = interpreter.MidRepresentation;
            var max = midRepresentation.Vars.VirtRegs.Max(vreg => vreg.Id) + 1;
            var cacheVariable = new LocalVariable()
            {
                FixedType = computedType,
                Id = max,
                Kind = VariableKind.Vreg
            };

            midRepresentation.Vars.VirtRegs.Add(cacheVariable);
            interpreter.AnalyzeProperties.RegisterVariable(cacheVariable);
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
            return (BinaryOperator) localOperation.Value;
        }

        public static FieldGetter GetFieldOperation(this List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetFieldOperation(localOperations, index);
        }

        public static FieldGetter GetFieldOperation(this List<LocalOperation> localOperations, int index)
        {
            return (FieldGetter) localOperations[index].Value;
        }

        public static BranchOperator GetBranchOperator(this List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetBranchOperator(localOperations, index);
        }

        public static BranchOperator GetBranchOperator(this List<LocalOperation> localOperations, int index)
        {
            return (BranchOperator) localOperations[index].Value;
        }

        public static UnaryOperator GetUnaryOperator(this List<LocalOperation> localOperations, List<int> calls, int i)
        {
            var index = calls[i];
            return GetUnaryOperator(localOperations, index);
        }

        public static UnaryOperator GetUnaryOperator(this List<LocalOperation> localOperations, int index)
        {
            return (UnaryOperator) localOperations[index].Value;
        }
    }
}