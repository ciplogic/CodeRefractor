#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class NewConstructedObject : LocalOperation
    {
        public MethodBase Info { get; set; }
        public LocalVariable AssignedTo { get; set; }

        public NewConstructedObject() : base(OperationKind.NewObject)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = new {1}()", AssignedTo.VarName, Info.DeclaringType.Name);
        }
    }
}