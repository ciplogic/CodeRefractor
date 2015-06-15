#region Uses

using System.Collections.Generic;
using CodeRefractor.CodeWriter.Platform;

#endregion

namespace CodeRefractor.CodeWriter.Linker
{
    public class LinkingData
    {
        public static readonly List<PlatformInvokeDllImports> Libraries = new List<PlatformInvokeDllImports>();
        public static int LibraryMethodCount;
        public GenerateTypeTableForIsInst IsInstTable = new GenerateTypeTableForIsInst();
        public StringTable Strings = new StringTable();

        public static bool SetInclude(string include)
        {
            if (string.IsNullOrWhiteSpace(include))
                return false;
            if (Includes.Contains(include))
                return false;
            Includes.Add(include);
            return true;
        }

        #region Singleton instance

        public static LinkingData Instance { get; } = new LinkingData();

        public static readonly HashSet<string> Includes = new HashSet<string>();

        #endregion
    }
}