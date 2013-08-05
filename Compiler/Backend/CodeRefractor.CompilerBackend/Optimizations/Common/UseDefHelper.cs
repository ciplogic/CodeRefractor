#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

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
                    result.AddUsage(((Assignment) operation.Value).Right);
                    break;

                case LocalOperation.Kinds.SetArrayItem:
                    AddUsagesOfSetArrayItem(operation, result);
                    break;
                case LocalOperation.Kinds.SetField:
                    AddUsagesOfSetField(operation, result);
                    break;
            }
            return result;
        }

        private static void AddUsagesOfSetField(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment) operation.Value;
            var arrayVar = (FieldSetter) assignment.Left;
            result.AddUsage(arrayVar.Instance);
            result.AddUsage(assignment.Right);
        }

        private static void AddUsagesOfSetArrayItem(LocalOperation operation, List<LocalVariable> result)
        {
            var assignment = (Assignment) operation.Value;
            var arrayVar = (ArrayVariable) assignment.Left;
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
    }
}