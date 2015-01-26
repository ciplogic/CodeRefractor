#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class RefAssignment : LocalOperation
    {
        public LocalVariable Left;
        public LocalVariable Right;

        public RefAssignment()
            : base(OperationKind.RefAssignment)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }
}