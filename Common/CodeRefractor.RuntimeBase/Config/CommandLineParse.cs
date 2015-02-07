#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Util;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.RuntimeBase.Config
{
    /**
     * A command line argument parser for the CodeRefractor compiler.
     */
    public class CommandLineParse
    {
        public readonly List<string> Arguments = new List<string>();

        public CommandLineParse()
        {
            InitializeOptimizationKindList(OptimizationKind.InFunction);
            InitializeOptimizationKindList(OptimizationKind.Global);
        }

        private static void InitializeOptimizationKindList(OptimizationKind optimizationKind)
        {
            OptimizationLevelBase.SortedOptimizations[optimizationKind] = new List<ResultingOptimizationPass>();
        }

        public void Process(string[] args)
        {
            Console.WriteLine("Code Refractor (version {0})", CodeRefractorVersions.Version);
            Arguments.AddRange(args);
            for (var i = 0; i < args.Length; i++)
            {
                var command = args[i];
                switch (command)
                {
                    case "-o":
                        ApplicationNativeExe = args[i + 1];
                        i++;
                        break;
                    case "-compiler":
                        i = HandleCompilerFlags(args, i);
                        break;
                    case "-CPP_FLAGS":
                        i = HandleCppFlags(args, i);
                        break;
                    case "--help":
                        ShowHelp();
                        break;
                    default:
                        ApplicationInputAssembly = command;
                        break;
                }
            }

            ApplicationInputAssembly = ApplicationInputAssembly.GetFullNameOfFile();
            SetOutputExeNameIfNoneSet();
        }

        private void SetOutputExeNameIfNoneSet()
        {
            if (!string.IsNullOrEmpty(ApplicationNativeExe)) return;
            var info = new FileInfo(ApplicationInputAssembly);
            var nameWithoutExtension = Path.Combine(info.DirectoryName, Path.GetFileNameWithoutExtension(info.Name));
            ApplicationNativeExe = nameWithoutExtension + "_cr.exe";
        }

        private static void ShowHelp()
        {
            var helpText =
                @" Code Refractor v {0}
 Created by Ciprian Khlud (c)
 Compiler - GPL2
 Runtime under Mit-X11

 CodeRefractor is a CIL to native compiler using C++ compiler as a backend

 Usage:
   CodeRefractor.Compiler.exe <input assembly> -o <output exe> [-compiler gcc|clang] [-CPP_FLAGS <compiler_flags>]
";
            helpText = string.Format(helpText, CodeRefractorVersions.Version);
            Console.WriteLine(helpText);
            Environment.Exit(0);
        }

        private static int HandleCompilerFlags(string[] args, int i)
        {
            if (i + 1 >= args.Length) // the compiler type was not given as the extra argument.
            {
                throw new ArgumentException("Using a compiler was specified (-compiler), but the name " +
                                            "of the compiler was not given: [-compiler gcc|clang]");
            }

            var compilerType = args[i + 1];
            NativeCompilationUtils.SetCompilerOptions(compilerType);
            return i + 1;
        }

        private static int HandleCppFlags(string[] args, int i)
        {
            NativeCompilationUtils.CompilerOptions.OptimizationFlags = args[i + 1];
            return i + 1;
        }


        public string OutputCpp = "output.cpp";
        public string OutputPro = "output.pro";
        public string ApplicationInputAssembly = "SimpleAdditions.exe";
        public string ApplicationNativeExe = string.Empty;
    }
}