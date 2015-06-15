#region Uses

using System.Collections.Generic;

#endregion

namespace CodeRefractor.CodeWriter.Platform
{
    public class PlatformInvokeDllImports
    {
        public readonly Dictionary<string, PlatformInvokeDllMethod> Methods =
            new Dictionary<string, PlatformInvokeDllMethod>();

        public PlatformInvokeDllImports(string dllName)
        {
            DllName = dllName;
        }

        public string DllName { get; set; }
    }
}