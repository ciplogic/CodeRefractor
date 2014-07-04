#region Uses

using System;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
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
            return String.Format("{0} = {1}", AssignedTo.Name, Right);
        }
    }
}