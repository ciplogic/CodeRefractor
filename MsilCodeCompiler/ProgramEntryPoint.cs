#region Usings

using System;
using System.IO;
using System.Reflection;
using CodeRefactor.OpenRuntime;
using CodeRefractor.ClosureCompute;
using CodeRefractor.Config;
using CodeRefractor.MiddleEnd.Optimizations.Util;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.Util;
using Ninject;
using Ninject.Extensions.Factory;

#endregion

namespace CodeRefractor.Compiler
{
    /**
     * The compiler main entry point. Sets up the Dependency Injection, 
     * and very basic error handling in case the application fails.
     */
    public static class ProgramEntryPoint
    {
        private static void Main(string[] args)
        {
            try
            {
                IKernel kernel = new StandardKernel(
                    new CodeRefractorNInjectModule()
                );

                var commandLineParse = kernel.Get<CommandLineParse>();
                commandLineParse.Process(args);

                OptimizationLevelBase.Instance = new OptimizationLevels();
                OptimizationLevelBase.OptimizerLevel = 3;
                kernel.Get<Program>().CallCompiler("");
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Calling the compiler failed: {0},\nStack trace: {1}",
                    e.Message, 
                    e.StackTrace);

                Console.ReadKey();

                Environment.Exit(1);
            }
        }
    }
}