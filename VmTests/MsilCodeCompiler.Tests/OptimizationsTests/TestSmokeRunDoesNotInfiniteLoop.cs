using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeRefactor.OpenRuntime;
using CodeRefractor.Backend.ProgramWideOptimizations.Virtual;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CompilerBackend.ProgramWideOptimizations;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.MiddleEnd.Optimizations.Util;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Optimizations;
using MsilCodeCompiler.Tests.Shared;
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
        [Test, Timeout(5000)]
        public void TestSimple()
        {
            // TODO: there must be a better way of setting up the whole CR test
            OptimizationLevelBase.Instance = new OptimizationLevels();
            OptimizationLevelBase.OptimizerLevel = 2;
            OptimizationLevelBase.Instance.EnabledCategories.Add(OptimizationCategories.All);

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
            var closureEntities = ClosureEntitiesUtils.BuildClosureEntities(
                                dotNetAssembly.EntryPoint, 
                                typeof (CrString).Assembly);

            var sourceCode = closureEntities.BuildFullSourceCode();

            Console.WriteLine(sourceCode);
        }
    }
}
