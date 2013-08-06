using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class FunctionPointerStore
    {
        public IdentifierValue AssignedTo { get; set; }

        public MethodBase FunctionPointer { get; set; }
    }
}