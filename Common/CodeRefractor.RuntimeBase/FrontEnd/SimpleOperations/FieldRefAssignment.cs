#region Uses

using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class FieldRefAssignment : LocalOperation
    {
        public FieldInfo Field;
        public LocalVariable Left;
        public LocalVariable Right;

        public FieldRefAssignment() : base(OperationKind.FieldRefAssignment)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left.Name, Right);
        }
    }
}