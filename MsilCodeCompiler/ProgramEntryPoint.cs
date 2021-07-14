#region Usings

using System;
using CodeRefractor.Backend;
using CodeRefractor.ClosureCompute;
using CodeRefractor.Config;
using CodeRefractor.MiddleEnd.Optimizations.Util;

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
                var commandLineParse = new CommandLineParse();
                commandLineParse.Process(args);

                OptimizationLevelBase.Instance = new OptimizationLevels();
                OptimizationLevelBase.OptimizerLevel = 3;
                new Program(commandLineParse, new ClosureEntitiesUtils(new ClosureEntities(new CppCodeGenerator()))).CallCompiler("");
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