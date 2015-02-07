using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CodeRefractor;
using CodeRefractor.Compiler;
using CodeRefractor.MiddleEnd.Optimizations.Util;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.Optimizations;
using MsilCodeCompiler.Tests.Shared;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.TestInfrastructure
{


    [TestFixture]
    class CodeTestingSampleWithGcc
    {
        [SetUp]
        public void Setup()
        {
            PathOfCompilerTools = @"C:\Oss\Dev-Cpp\MinGW64\bin\";
            CompilerExe = "g++.exe";

            OptimizationLevelBase.Instance = new OptimizationLevels();
            OptimizationLevelBase.OptimizerLevel = 2;
            OptimizationLevelBase.Instance.EnabledCategories.Add(OptimizationCategories.All);
        }

        private static string BuildArgs(string outputFile)
        {
            var sbArgs = new StringBuilder();
            sbArgs.Append(outputFile);
            sbArgs.Append("  -std=c++11 -c ");
            return sbArgs.ToString();
        }


        [Test]
        public void TestCodeInMainIfCompiles()
        {
            var mainBody =
                @"var a = 30.0;
            var b = 9.0 - (int)a / 5;
           
            Console.WriteLine(b);";
            var fullCode = CompilingProgramBase.GenerateMainCode(mainBody);

            var csAssembly = CompilingProgramBase.CompileSource(fullCode);
            Assert.IsNotNull(csAssembly);
            var outputFile = Program.CallCompiler(csAssembly.Location);
            
            var pathToGpp = Path.Combine(PathOfCompilerTools, CompilerExe);
            //var outputFile =  @"c:\Oss\ClClean\bin\output.cpp";
            var arguments = BuildArgs(outputFile);
            var resultCommand = pathToGpp.ExecuteCommand(arguments);
            Assert.IsTrue(!resultCommand.Contains("error"));
        }

        public string CompilerExe { get; set; }

        public string PathOfCompilerTools { get; set; }
    }
}
