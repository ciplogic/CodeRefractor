#region Uses

using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class FunctionPointerStore : LocalOperation
    {
        public FunctionPointerStore()
            : base(OperationKind.LoadFunction)
        {
        }

        public IdentifierValue AssignedTo { get; set; }
        public MethodBase FunctionPointer { get; set; }
        public MethodInfo CustomData { get; set; }

        public override string ToString()
        {
            return $"{AssignedTo} = {FunctionPointer}";
        }
    }
}