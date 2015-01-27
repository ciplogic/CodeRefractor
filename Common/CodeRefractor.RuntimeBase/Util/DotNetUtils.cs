#region Usings

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace CodeRefractor.RuntimeBase.Util
{
    public static class DotNetUtils
    {
        static DotNetUtils()
        {
            DotNetPath = RuntimeEnvironment.GetRuntimeDirectory();
        }

        public static string DotNetPath { get; private set; }

        public static string CallDotNetCommand(string program, string arguments, string outputDirectory = "")
        {
            var ngenApp = Path.Combine(DotNetPath, program);
            return ngenApp.ExecuteCommand(arguments, outputDirectory);
        }
    }
}