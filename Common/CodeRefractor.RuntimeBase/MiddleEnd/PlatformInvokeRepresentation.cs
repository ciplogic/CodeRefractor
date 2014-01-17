#region Usings

using System.Runtime.InteropServices;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class PlatformInvokeRepresentation
    {
        public string EntryPoint { get; set; }
        public string LibraryName { get; set; }
        
    }
}