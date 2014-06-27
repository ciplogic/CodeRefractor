#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
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
            return string.Format("{0} = {1}", AssignedTo, FunctionPointer);
        }
    }
}