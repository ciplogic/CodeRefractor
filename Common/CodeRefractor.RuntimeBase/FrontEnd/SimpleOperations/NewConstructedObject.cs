#region Uses

using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class NewConstructedObject : LocalOperation
    {
        public NewConstructedObject() : base(OperationKind.NewObject)
        {
        }

        public MethodBase Info { get; set; }
        public LocalVariable AssignedTo { get; set; }

        public override string ToString()
        {
            return $"{AssignedTo.VarName} = new {Info.DeclaringType.Name}()";
        }
    }
}