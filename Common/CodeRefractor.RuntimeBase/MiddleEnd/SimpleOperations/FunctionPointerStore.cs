#region Usings

using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class FunctionPointerStore
    {
        public IdentifierValue AssignedTo { get; set; }

        public MethodBase FunctionPointer { get; set; }
    }
}