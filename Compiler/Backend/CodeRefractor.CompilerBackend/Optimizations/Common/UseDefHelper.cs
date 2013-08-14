#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

#endregion

namespace CodeRefractor.CompilerBackend.Optimizations.Common
{
    public static class UseDefHelper
    {
        public static LocalOperation GetNextUsage(this List<LocalOperation> operations, LocalVariable variable,
                                                  int startPos, out int pos)
        {
            pos = startPos;
            for (var i = startPos; i < operations.Count; i++)
            {
                var operation = operations[i];
                pos = i;
                if (operation.IsBranchOperation())
                    return null;
                if (OperationUses(operation, variable))
                    return operation;
            }
            return null;
        }

        public static List<LocalVariable> GetUsages(this LocalOperation operation)
        {
            var result = new List<LocalVariable>();
            switch (operation.Kind)
            {
                case LocalOperation.Kinds.Assignment:
                    result.AddUsage(((Assignment)operation.Value).Right);
                    break;
                case LocalOperation.Kinds.UnaryOperator:
                    var unaryOperator = ((UnaryOperator)operation.Value);
                    result.AddUsage(unaryOperator.Left);
                    break;
                case LocalOperation.Kinds.BranchOperator:
                    var branchOperator = ((BranchOperator)operation.Value);
                    result.AddUsage(branchOperator.CompareValue);
                    break;
                case LocalOperation.Kinds.BinaryOperator:
                    var binaryOperator = ((BinaryOperator)operation.Value);
                    result.AddUsage(binaryOperator.Left);
                    result.AddUsage(binaryOperator.Right);
                    break;
                case LocalOperation.Kinds.GetArrayItem:
                    AddUsagesOfGetArrayItem(operation, result);
                    break;
                case LocalOperation.Kinds.SetArrayItem:
                    AddUsagesOfSetArrayItem(operation, result);
                    break;


                case LocalOperation.Kinds.GetField:
                    AddUsagesOfGetField(operation, result);
                    break;
                case LocalOperation.Kinds.SetField:
                    AddUsagesOfSetField(operation, result);
                    break;
            }
            return result;
        }

        private static void AddUsagesOfGetField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (FieldGetter)assignment.Right;
            result.AddUsage(arrayVar.Instance);
        }
        private static void AddUsagesOfSetField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (FieldSetter)assignment.AssignedTo;
            result.AddUsage(arrayVar.Instance);
            result.AddUsage(assignment.Right);
        }

        private static void AddUsagesOfGetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (ArrayVariable)assignment.Right;
            result.AddUsage(arrayVar.Parent);
            result.AddUsage(arrayVar.Index);
        }


        private static void AddUsagesOfSetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment)operation.Value;
            var arrayVar = (ArrayVariable)assignment.AssignedTo;
            result.AddUsage(arrayVar.Parent);
            result.AddUsage(arrayVar.Index);
            result.AddUsage(assignment.Right);
        }

        public static bool OperationUses(this LocalOperation operation, LocalVariable variable)
        {
            var result = GetUsages(operation);

            return result.Any(localVariable => localVariable.Equals(variable));
        }

        private static void AddUsage(this List<LocalVariable> usages, IdentifierValue usage)
        {
            var localVar = usage as LocalVariable;
            if (localVar == null)
                return;
            usages.Add(localVar);
        }

        public static LocalVariable GetUseDefinition(this LocalOperation operation)
        {
            var kind = operation.Kind;
            switch (kind)
            {
                    case LocalOperation.Kinds.Assignment:
                    var assign = operation.GetAssignment();
                    return assign.AssignedTo;
                    case LocalOperation.Kinds.BinaryOperator:
                    var binOp = (BinaryOperator)operation.Value;
                    return binOp.AssignedTo;
                    case LocalOperation.Kinds.UnaryOperator:
                    var unOp = (UnaryOperator)operation.Value;
                    return unOp.AssignedTo;

                default:
                    return null;
            }
        }

        public static HashSet<int> GetVariableUsages(this List<LocalOperation> localOperations, LocalVariable variable)
        {
            var result = new HashSet<int>();
            var pos = -1;
            foreach (var op in localOperations)
            {
                pos++;

                if (op.OperationUses(variable))
                    result.Add(pos);

            }
            return result;
        }
    }
}