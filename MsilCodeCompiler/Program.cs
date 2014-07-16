#region Usings

using System;
using System.IO;
using System.Reflection;
using CodeRefactor.OpenRuntime;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Optimizations.Util;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Compiler
{
    public static class Program
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

            var closureEntities = ClosureEntitiesUtils.BuildClosureEntities(definition, typeof(CrString).Assembly);

            var sb = closureEntities.BuildFullSourceCode();
            var end = Environment.TickCount - start;
            Console.WriteLine("Compilation time: {0} ms", end);
            var fullPath = commandLineParse.OutputCpp.GetFullFileName();
            sb.ToFile(fullPath);
            NativeCompilationUtils.CompileAppToNativeExe(commandLineParse.OutputCpp,
                                                         commandLineParse.ApplicationNativeExe);
        }
        private static void Main(string[] args)
        {
            var commandLineParse = CommandLineParse.Instance;
            commandLineParse.Process(args);


            OptimizationLevelBase.Instance = new OptimizationLevels();
            OptimizationLevelBase.OptimizerLevel = 2;
            OptimizationLevelBase.Instance.EnabledCategories.Add(OptimizationCategories.All);
            CallCompiler("", "");
        }
    }
}