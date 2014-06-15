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

        public override string ToString()
        {
            return string.Format("{0} = {1}", AssignedTo.ToString(), FunctionPointer.ToString());
        }
    }
}