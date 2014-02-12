#region Usings

using System.Collections.Generic;

#endregion

namespace Compiler.CodeWriter.Platform
{
    public class PlatformInvokeDllImports
    {
        public PlatformInvokeDllImports(string dllName)
        {
            DllName = dllName;
        }

        public string DllName { get; set; }

        public readonly Dictionary<string, PlatformInvokeDllMethod> Methods =
            new Dictionary<string, PlatformInvokeDllMethod>();
    }
}