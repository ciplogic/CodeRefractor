#region Usings

using System.Collections.Generic;

#endregion

namespace CodeRefractor.Compiler.Backend
{
    internal class PlatformInvokeDllImports
    {
        public PlatformInvokeDllImports(string dllName)
        {
            DllName = dllName;
        }

        public string DllName { get; set; }
        public Dictionary<string, string> Methods = new Dictionary<string, string>();
    }
}