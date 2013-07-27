#region Usings

using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

#endregion

namespace CodeRefractor.Compiler.Util
{
    public static class DotNetUtils
    {
        static DotNetUtils()
        {
            DotNetPath = RuntimeEnvironment.GetRuntimeDirectory();
        }

        public static string DotNetPath { get; private set; }

        public static void CallDotNetCommand(string program, string arguments, string outputDirectory = "")
        {
            var ngenApp = Path.Combine(DotNetPath, program);
            var proces = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = ngenApp,
                    WorkingDirectory =
                        string.IsNullOrEmpty(outputDirectory)
                            ? DotNetPath
                            : outputDirectory,
                    Arguments = arguments,
                    WindowStyle = ProcessWindowStyle.Hidden,
#if DEBUG
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
#endif
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            proces.Start();
            proces.WaitForExit();

#if DEBUG
            var standardOutput = proces.StandardOutput.ReadToEnd();
            var standardError = proces.StandardError.ReadToEnd();

            Debug.WriteLine(standardOutput);
            Debug.WriteLine(standardError);
#endif
        }
    }
}