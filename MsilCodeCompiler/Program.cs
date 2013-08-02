#region Usings

using CodeRefractor.Compiler.Config;
using CodeRefractor.Compiler.Util;

#endregion

namespace CodeRefractor.Compiler
{
    internal static class Program
    {
        public static CrRuntimeLibrary CrCrRuntimeLibrary;

        private static void Main(string[] args)
        {
            CrRuntimeLibrary.DefaultSetup();
            var commandLineParse = CommandLineParse.Instance;
            commandLineParse.Process(args);
            //MetaLinker.ScanAssembly(typeof(int));
            //MetaLinker.ScanAssembly(typeof(Console));
            NativeCompilationUtils.SetCompilerOptions("gcc");
            CommandLineParse.OptimizerLevel = 1;
            NativeCompilationUtils.CallCompiler("", "");
            //var standardOutput = applicationNativeExe.ExecuteCommand();
            //Console.WriteLine(standardOutput);
            //Console.ReadKey();
        }
    }
}