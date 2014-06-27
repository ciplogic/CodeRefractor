#region Usings

using System.Collections.Generic;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.RuntimeBase.Backend.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.RuntimeBase.Optimizations;
using MsilCodeCompiler.Tests.Shared;
using NUnit.Framework;

#endregion

namespace MsilCodeCompiler.Tests.OptimizationsTests
{
    [TestFixture]
    public class TestConstantVariableOperatorPropagationTests : CompilingProgramBase
    {
        [Test]
        public void TestSimple()
        {
            var optimizations = new List<ResultingOptimizationPass>
            {
                new ConstantVariableOperatorPropagation()
            };
            var mainBody =
                @"var a = 30.0;
            var b = 9.0 - (int)a / 5;
           
            Console.WriteLine(b);";
            TryCSharpMain(mainBody, optimizations);
        }
    }
}