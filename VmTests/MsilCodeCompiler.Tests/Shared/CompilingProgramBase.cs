using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeRefractor.Compiler;
using CodeRefractor.Compiler.Backend;
using CodeRefractor.Compiler.Optimizations;
using CodeRefractor.Compiler.Optimizations.Common;
using CodeRefractor.Compiler.Util;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Optimizations;
using NUnit.Framework;

namespace MsilCodeCompiler.Tests.Shared
{
    public class CompilingProgramBase
    {
        protected bool EvalCSharpMainToNative(string bodyOfMain, List<OptimizationPass> optimizationPasses = null)
        {
            var code = String.Format(@"
using System;
class C {{ 
    public static void Main() {{{0}}} 
}}
", bodyOfMain);
            return EvaluateCSharpToNative(code, optimizationPasses);
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
                           //new DceVRegAssigned(),
                           //new VRegVariablePropagation(),
                       }.ToList();
        } 
        protected bool EvaluateCSharpToNative(string code, List<OptimizationPass> optimizationPasses=null)
        {
            const string applicationNativeExe = "a_test.exe";
            const string outputCpp = "output.cpp";
            var assembly = CompileSource(code);
            Assert.IsNotNull(assembly);
            var start = Environment.TickCount;
            var expectedInput = assm.ExecuteCommand();
            var end = Environment.TickCount - start;
            Console.WriteLine("Time1: {0}", end);
            if (optimizationPasses == null)
                optimizationPasses = DefaultOptimizationPasses();

            var crRuntimeLibrary = new CrRuntimeLibrary();
            var linker = assembly.EntryPoint.CreateLinkerFromEntryPoint(optimizationPasses);

            
            var generatedSource = CppCodeGenerator.BuildFullSourceCode(linker, crRuntimeLibrary);
            generatedSource.ToFile(outputCpp);
            NativeCompilationUtils.CompileAppToNativeExe(outputCpp, applicationNativeExe);
            code.DeleteFile();

            var startCpp = Environment.TickCount;
            var actualOutput = applicationNativeExe.ExecuteCommand();
            var endCpp = Environment.TickCount - startCpp;
            Console.WriteLine("Time1: {0}", endCpp);
            applicationNativeExe.DeleteFile();
            return actualOutput == expectedInput;
        }
    }
}