#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeRefractor;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.Util;
using NUnit.Framework;

#endregion

namespace MsilCodeCompiler.Tests.Shared
{
    public class CompilingProgramBase
    {
        protected bool EvalCSharpMain(string bodyOfMain, List<ResultingOptimizationPass> optimizationPasses = null)
        {
            var code = $@"
using System;
class C {{ 
    public static void Main() {{{bodyOfMain}}} 
}}
";
            return EvaluateCSharpToNative(code, optimizationPasses);
        }

        protected bool TryCSharpMain(string bodyOfMain, List<ResultingOptimizationPass> optimizationPasses = null)
        {
            var code = GenerateMainCode(bodyOfMain);
            return TryCompileCSharp(code, optimizationPasses);
        }

        public static string GenerateMainCode(string bodyOfMain)
        {
            var code = $@"
using System;
class C {{ 
    public static void Main() {{{bodyOfMain}}} 
}}
";
            return code;
        }

        /**
         * Compiles the given source, and returns the compiled assembly.
         * The Assembly will be a file on the disk generated in the current working folder.
         */
        public static Assembly CompileSource(string source)
        {
            const string dummyName = "dump.cs";
            File.WriteAllText(dummyName, source);
            var currentDirectory = Directory.GetCurrentDirectory();

            var output = DotNetUtils.CallDotNetCommand("csc.exe", "/out:dump.exe dump.cs", currentDirectory);
            if (output.Contains("error"))
            {
                return null;
            }

            var assm = Path.Combine(currentDirectory, "dump.exe");
            dummyName.DeleteFile();

            // TODO: dump.exe leaks exe files.
            // TODO: parallel execution, requires generated names instead of dump.exe.

            return Assembly.LoadFile(assm);
        }

        string assm;

        public static List<ResultingOptimizationPass> DefaultOptimizationPasses()
        {
            return new ResultingOptimizationPass[]
            {
                //new VRegConstantFolding(),
                //new DceVRegUnused(),
                //new VRegVariablePropagation(),
            }.ToList();
        }

        protected bool EvaluateCSharpToNative(string code, List<ResultingOptimizationPass> optimizationPasses = null)
        {
            string expectedInput;
            var outputCpp = GenerateOutputCppFromCode(code, optimizationPasses, out expectedInput);
            const string applicationNativeExe = "a_test.exe";
            NativeCompilationUtils.CompileAppToNativeExe(outputCpp, applicationNativeExe);
            code.DeleteFile();

            var startCpp = Environment.TickCount;
            var actualOutput = applicationNativeExe.ExecuteCommand(string.Empty, Directory.GetCurrentDirectory());
            var endCpp = Environment.TickCount - startCpp;
            Console.WriteLine("Time1: {0}", endCpp);
            applicationNativeExe.DeleteFile();
            return actualOutput == expectedInput;
        }

        protected bool TryCompileCSharp(string code, List<ResultingOptimizationPass> optimizationPasses = null)
        {
            string expectedInput;
            var outputCpp = GenerateOutputCppFromCode(code, optimizationPasses, out expectedInput);
            var result = File.Exists(outputCpp);
            code.DeleteFile();
            return result;
        }

        string GenerateOutputCppFromCode(string code, List<ResultingOptimizationPass> optimizationPasses,
            out string expectedInput)
        {
            const string outputCpp = "output.cpp";
            var assembly = CompileSource(code);
            Assert.IsNotNull(assembly);

            assm = assembly.Location;

            var start = Environment.TickCount;
            expectedInput = assm.ExecuteCommand("", Directory.GetCurrentDirectory());
            var end = Environment.TickCount - start;
            Console.WriteLine("Time1: {0}", end);
            if (optimizationPasses == null)
                optimizationPasses = DefaultOptimizationPasses();



            //var generatedSource = CppCodeGenerator.BuildFullSourceCode(linker);
            //generatedSource.ToFile(outputCpp);
            return outputCpp;
        }
    }
}