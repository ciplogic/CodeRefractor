#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeRefractor.Compiler;
using CodeRefractor.CompilerBackend;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.Util;
using NUnit.Framework;

#endregion

namespace MsilCodeCompiler.Tests.Shared
{
    public class CompilingProgramBase
    {
        protected bool EvalCSharpMain(string bodyOfMain, List<OptimizationPass> optimizationPasses = null)
        {
            var code = String.Format(@"
using System;
class C {{ 
    public static void Main() {{{0}}} 
}}
", bodyOfMain);
            return EvaluateCSharpToNative(code, optimizationPasses);
        }

        protected bool TryCSharpMain(string bodyOfMain, List<OptimizationPass> optimizationPasses = null)
        {
            var code = String.Format(@"
using System;
class C {{ 
    public static void Main() {{{0}}} 
}}
", bodyOfMain);
            return TryCompileCSharp(code, optimizationPasses);
        }

        private Assembly CompileSource(string source)
        {
            const string dummyName = "dump.cs";
            File.WriteAllText(dummyName, source);
            var currentDirectory = Directory.GetCurrentDirectory();
            DotNetUtils.CallDotNetCommand("csc.exe", "dump.cs", currentDirectory);
            assm = Path.Combine(currentDirectory, "dump.exe");
            dummyName.DeleteFile();
            return Assembly.LoadFile(assm);
        }

        private string assm;

        public static List<OptimizationPass> DefaultOptimizationPasses()
        {
            return new OptimizationPass[]
            {
                //new VRegConstantFolding(),
                //new DceVRegUnused(),
                //new VRegVariablePropagation(),
            }.ToList();
        }

        protected bool EvaluateCSharpToNative(string code, List<OptimizationPass> optimizationPasses = null)
        {
            string expectedInput;
            var outputCpp = GenerateOutputCppFromCode(code, optimizationPasses, out expectedInput);
            const string applicationNativeExe = "a_test.exe";
            NativeCompilationUtils.CompileAppToNativeExe(outputCpp, applicationNativeExe);
            code.DeleteFile();

            var startCpp = Environment.TickCount;
            var actualOutput = applicationNativeExe.ExecuteCommand();
            var endCpp = Environment.TickCount - startCpp;
            Console.WriteLine("Time1: {0}", endCpp);
            applicationNativeExe.DeleteFile();
            return actualOutput == expectedInput;
        }

        protected bool TryCompileCSharp(string code, List<OptimizationPass> optimizationPasses = null)
        {
            string expectedInput;
            var outputCpp = GenerateOutputCppFromCode(code, optimizationPasses, out expectedInput);
            var result = File.Exists(outputCpp);
            code.DeleteFile();
            return result;
        }

        private string GenerateOutputCppFromCode(string code, List<OptimizationPass> optimizationPasses,
            out string expectedInput)
        {
            const string outputCpp = "output.cpp";
            var assembly = CompileSource(code);
            Assert.IsNotNull(assembly);
            var start = Environment.TickCount;
            expectedInput = assm.ExecuteCommand();
            var end = Environment.TickCount - start;
            Console.WriteLine("Time1: {0}", end);
            if (optimizationPasses == null)
                optimizationPasses = DefaultOptimizationPasses();

            var crRuntimeLibrary = new CrRuntimeLibrary();


            //var generatedSource = CppCodeGenerator.BuildFullSourceCode(linker);
            //generatedSource.ToFile(outputCpp);
            return outputCpp;
        }
    }
}