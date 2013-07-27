#region Usings

using System.Collections.Generic;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class PlatformInvokeRepresentation
    {
        public readonly List<ArgumentVariable> Arguments = new List<ArgumentVariable>();
        public string LibraryName { get; set; }
        public string MethodName { get; set; }
    }
}