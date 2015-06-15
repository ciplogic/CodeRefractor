#region Usings

using System;
using CodeRefractor.Config;
using CodeRefractor.MiddleEnd.Optimizations.Util;
using CodeRefractor.RuntimeBase.Config;
using Ninject;

#endregion

namespace CodeRefractor.Compiler
{
    /**
     * The compiler main entry point. Sets up the Dependency Injection, 
     * and very basic error handling in case the application fails.
     */
    public static class ProgramEntryPoint
    {
        static void Main(string[] args)
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