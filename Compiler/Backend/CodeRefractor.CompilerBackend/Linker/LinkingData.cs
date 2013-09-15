using System.Collections.Generic;
using System.Text;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.CompilerBackend.OuputCodeWriter.Platform;

namespace CodeRefractor.CompilerBackend.Linker
{
    class LinkingData
    {
        public static readonly List<PlatformInvokeDllImports> Libraries = new List<PlatformInvokeDllImports>();
        public static int LibraryMethodCount;

        public StringTable Strings =new StringTable();

        #region Singleton instance
        static readonly LinkingData StaticInstance = new LinkingData();
        public static LinkingData Instance {get { return StaticInstance; }}

        public static readonly HashSet<string> Includes = new HashSet<string>();
        #endregion

        public static bool SetInclude(string include)
        {
            if (Includes.Contains(include))
                return false;
            Includes.Add(include);
            return true;
        }
    }
}
