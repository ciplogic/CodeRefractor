using System;
using CodeRefactor.OpenRuntime;
using CodeRefractor.ClosureCompute;
using CodeRefractor.Config;
using CodeRefractor.MiddleEnd.Optimizations.Util;
using CodeRefractor.Optimizations;
using MsilCodeCompiler.Tests.Shared;
using Ninject;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.OptimizationsTests
{
    /// <summary>
    /// This test checks if a regular run of a very simple compilation doesn't infinite
    /// loop on some optimisations.
    /// This addresses issue #12.
    /// </summary>
    [TestFixture]
    public class TestSmokeRunDoesNotInfiniteLoop
    {
        StandardKernel kernel;

        [Test, Timeout(5000)]
        public void TestSimple()
        {
            // TODO: there must be a better way of setting up the whole CR test
            OptimizationLevelBase.Instance = new OptimizationLevels();
            OptimizationLevelBase.OptimizerLevel = 2;
            OptimizationLevelBase.Instance.EnabledCategories.Add(OptimizationCategories.All);

            kernel = new StandardKernel(
                new CodeRefractorNInjectModule()
            );

            var cSharpCode =
                @"
                    public class C
                    {
                        public static void Main()
                        {
                            var message = ""test"";
                            message += ""other"";

                            System.Console.WriteLine(""main entered"");
                        }
                    }
                ";

            var dotNetAssembly = CompilingProgramBase.CompileSource(cSharpCode);
            var closureEntities = kernel.Get<ClosureEntitiesUtils>()
                        .BuildClosureEntities(
                                dotNetAssembly.EntryPoint, 
                                typeof (CrString).Assembly);

            var sourceCode = closureEntities.BuildFullSourceCode();

            Console.WriteLine(sourceCode.Src);
        }
    }
}
