#region Uses

using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

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
            return $"{Left.Name} = {Right}";
        }
    }
}