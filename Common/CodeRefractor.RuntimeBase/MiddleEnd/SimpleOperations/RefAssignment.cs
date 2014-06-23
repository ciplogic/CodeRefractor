#region Usings

using System;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class RefAssignment : BaseOperation
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

    public class FieldRefAssignment : BaseOperation
    {
        public LocalVariable Left;
        public LocalVariable Right;
        public FieldInfo Field;

        public FieldRefAssignment() : base(OperationKind.FieldRefAssignment)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }
}