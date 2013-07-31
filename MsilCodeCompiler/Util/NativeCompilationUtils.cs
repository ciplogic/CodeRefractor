#region Usings

using System;
using System.IO;
using System.Reflection;
using CodeRefractor.Compiler.Backend;
using CodeRefractor.Compiler.Config;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.DataBase.SerializeXml;

#endregion

namespace CodeRefractor.Compiler.Util
{
    public static class NativeCompilationUtils
    {
        private class ClangOptions : Options
        {
            public ClangOptions()
            {
                PathOfCompilerTools = @"c:\Oss\clang_llvm-3.2\bin\";
                CompilerExe = "clang++.exe";
                OptimizationFlags = "-O2 ";
            }
        }

        private class GccOptions : Options
        {
            public GccOptions()
            {
                PathOfCompilerTools = @"c:\Oss\Dev-Cpp\MinGW64\bin\";
                CompilerExe = "g++.exe";
                OptimizationFlags = "-Ofast -fomit-frame-pointer -ffast-math -std=c++11 -static-libgcc ";
                LinkerOptions = "";
            }
        }

        private class LinuxGccOptions : Options
        {
            public LinuxGccOptions()
            {
                PathOfCompilerTools = @"";
                CompilerExe = "g++";
                OptimizationFlags = "-Ofast -fomit-frame-pointer -ffast-math -std=c++11 -static-libgcc ";
                LinkerOptions = "-L. -lCRRuntimeLinux -ldl";
            }
        }

        [XNode]
        public class Options
        {
            public string CompilerKind;
            public string PathOfCompilerTools;
            public string CompilerExe;
            public string OptimizationFlags;

            public string LinkerOptions;
        }

        public static Options CompilerOptions = new LinuxGccOptions();

        public static void SetCompilerOptions(string compilerKind)
        {
            switch (compilerKind)
            {
                case "gcc":
                    CompilerOptions = new GccOptions();
                    break;
                case "linx_gcc":
                    CompilerOptions = new LinuxGccOptions();
                    break;
                case "clang":
                    CompilerOptions = new ClangOptions();
                    break;
                case "msvc":
                    CompilerOptions = new ClangOptions();
                    break;
            }
        }

        public static void CallCompiler(string inputAssemblyName, string outputExeName)
        {
            var commandLineParse = CommandLineParse.Instance;
            if (!string.IsNullOrEmpty(inputAssemblyName))
            {
                commandLineParse.ApplicationInputAssembly = inputAssemblyName;
            }
            if (!string.IsNullOrEmpty(outputExeName))
            {
                commandLineParse.ApplicationNativeExe = outputExeName;
            }
            var dir = Directory.GetCurrentDirectory();
            inputAssemblyName = Path.Combine(dir, commandLineParse.ApplicationInputAssembly);
            var asm = Assembly.LoadFile(inputAssemblyName);
            var definition = asm.EntryPoint;
            var start = Environment.TickCount;
            var linker = definition.CreateLinkerFromEntryPoint(commandLineParse.BuildOptimizationPasses2());

            var sb = CppCodeGenerator.BuildFullSourceCode(linker, Program.CrCrRuntimeLibrary);
            var end = Environment.TickCount - start;
            Console.WriteLine("Compilation time: {0} ms", end);

            sb.ToFile(commandLineParse.OutputCpp);
            CompileAppToNativeExe(commandLineParse.OutputCpp, commandLineParse.ApplicationNativeExe);
        }

        public static void CompileAppToNativeExe(string outputCpp, string applicationNativeExe)
        {
            var pathToGpp = CompilerOptions.PathOfCompilerTools + CompilerOptions.CompilerExe;

            var commandLineFormat = "{0} " + CompilerOptions.OptimizationFlags + " {2} -o {1}";

            var arguments = String.Format(commandLineFormat, outputCpp, applicationNativeExe,
                CompilerOptions.LinkerOptions);
            var standardOutput = pathToGpp.ExecuteCommand(arguments);
            if (!String.IsNullOrWhiteSpace(standardOutput))
            {
                throw new InvalidOperationException(String.Format("Errors when compiling: {0}", standardOutput));
            }
            (CompilerOptions.PathOfCompilerTools + "strip").ExecuteCommand(applicationNativeExe);
        }
    }
}