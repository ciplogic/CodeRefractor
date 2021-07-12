#region Uses

using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class StaticFieldRefAssignment : LocalOperation
    {
        public FieldInfo Field;
        public LocalVariable Left;

        public StaticFieldRefAssignment()
            : base(OperationKind.StaticFieldRefAssignment)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left.Name);
        }
    }
}