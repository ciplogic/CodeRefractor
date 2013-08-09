#region Usings

using System.Runtime.InteropServices;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public class PlatformInvokeRepresentation
    {
        public string LibraryName { get; set; }
        public string MethodName { get; set; }

        public CallingConvention CallingConvention { get; set; }

        public string EntryPoint { get; set; }
    }
}