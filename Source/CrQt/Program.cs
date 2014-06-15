using System;
using System.IO;
using System.Reflection;
using CodeRefactor.OpenRuntime;
using CodeRefractor.CompilerBackend.Optimizations.Util;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations.ConstParameters;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations.Virtual;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Backend.ProgramWideOptimizations;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.Util;

namespace CrQt
{
    class Program
    {
        public static void CallCompiler(string inputAssemblyName, string outputExeName)
        {
            var commandLineParse = CommandLineParse.Instance;
            if (!String.IsNullOrEmpty(inputAssemblyName))
            {
                commandLineParse.ApplicationInputAssembly = inputAssemblyName;
            }
            if (!String.IsNullOrEmpty(outputExeName))
            {
                commandLineParse.ApplicationNativeExe = outputExeName; 
                commandLineParse.OutputCpp = Path.ChangeExtension(commandLineParse.ApplicationNativeExe, ".cpp");

            }
            var dir = Directory.GetCurrentDirectory();
            inputAssemblyName = Path.Combine(dir, commandLineParse.ApplicationInputAssembly);
            var asm = Assembly.LoadFile(inputAssemblyName);
            var definition = asm.EntryPoint;
            var start = Environment.TickCount;


            var crRuntime = new CrRuntimeLibrary();
            crRuntime.ScanAssembly(typeof(CrString).Assembly);

            var programClosure = new ProgramClosure(definition, crRuntime);

            var sb = programClosure.BuildFullSourceCode(programClosure.Runtime);
            var end = Environment.TickCount - start;
            Console.WriteLine("Compilation time: {0} ms", end);

            sb.ToFile(commandLineParse.OutputCpp);
            NativeCompilationUtils.CompileAppToNativeExe(commandLineParse.OutputCpp,
                                                         commandLineParse.ApplicationNativeExe);
        }

        static void Main(string[] args)
        {
            var commandLineParse = CommandLineParse.Instance;
            commandLineParse.Process(args);


            OptimizationLevelBase.Instance = new OptimizationLevels();
            NativeCompilationUtils.SetCompilerOptions("gcc");
            CommandLineParse.OptimizerLevel = 2;
            CallCompiler("", "");
        }
    }
}
