#region Usings

using System.Collections.Generic;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter.Platform
{
    internal class PlatformInvokeDllImports
    {
        public PlatformInvokeDllImports(string dllName)
        {
            DllName = dllName;
        }

        public string DllName { get; set; }
        public readonly Dictionary<string, PlatformInvokeDllMethod> Methods = new Dictionary<string, PlatformInvokeDllMethod>();
    }
}