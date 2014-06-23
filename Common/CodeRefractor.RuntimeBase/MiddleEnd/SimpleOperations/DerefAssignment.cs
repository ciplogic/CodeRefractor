#region Usings

using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class DerefAssignment : BaseOperation
    {
        public LocalVariable Left;
        public LocalVariable Right;

        public DerefAssignment()
            : base(OperationKind.DerefAssignment)
        {
        }

        public override string ToString()
        {
            return String.Format("{0} = {1}", Left.Name, Right);
        }
    }
}