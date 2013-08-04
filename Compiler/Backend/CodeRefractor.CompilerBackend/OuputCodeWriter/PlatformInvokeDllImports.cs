#region Usings

using System.Collections.Generic;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    internal class PlatformInvokeDllImports
    {
        public PlatformInvokeDllImports(string dllName)
        {
            DllName = dllName;
        }

        public string DllName { get; set; }
        public readonly Dictionary<string, string> Methods = new Dictionary<string, string>();
    }
}