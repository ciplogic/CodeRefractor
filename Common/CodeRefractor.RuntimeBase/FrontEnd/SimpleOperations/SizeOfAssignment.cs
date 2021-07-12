#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class SizeOfAssignment : LocalOperation
    {
        public LocalVariable AssignedTo;
        public Type Right;

        public SizeOfAssignment()
            : base(OperationKind.SizeOf)
        {
        }

        public override string ToString()
        {
            return $"{AssignedTo.Name} = {Right}";
        }
    }
}