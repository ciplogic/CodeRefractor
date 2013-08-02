#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CodeRefractor.Compiler.Optimizations;
using CodeRefractor.Compiler.Optimizations.ConstantDfa;
using CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation;
using CodeRefractor.Compiler.Optimizations.ConstantFoldingAndPropagation.ComplexAssignments;
using CodeRefractor.Compiler.Optimizations.Jumps;
using CodeRefractor.Compiler.Optimizations.ReachabilityDfa;
using CodeRefractor.Compiler.Optimizations.SimpleDce;
using CodeRefractor.Compiler.Util;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.Compiler.Config
{
    internal class CommandLineParse
    {
        public static readonly CommandLineParse StaticInstance = new CommandLineParse();
        public readonly List<string> Arguments = new List<string>();

        public static CommandLineParse Instance
        {
            get { return StaticInstance; }
        }

        public void Process(string[] args)
        {
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
        public string ApplicationInputAssembly = "SimpleAdditions.exe";
        public string ApplicationNativeExe = string.Empty;
        public static List<OptimizationPass> OptimizationPasses;
        private static int _optimizerLevel;


        public static List<OptimizationPass> BuildOptimizationPasses2()
        {
            return new OptimizationPass[]
            {
                new DeleteGappingVregAssignment(),
                
                new ReachabilityLines(),
                new ConstantDfaAnalysis(),
                
                new EvaluatePureFunctionWithConstantCall(),
                new VRegVariablePropagation()
            }.ToList();
        }


        public static List<OptimizationPass> BuildOptimizationPasses1()
        {
            return new OptimizationPass[]
                                         {
                                             new DoubleAssignPropagation(), 
                                             new AssignToReturnPropagation(), 
                                             new DeadStoreLastSequenceRemover(), 

                                             new DceLocalAssigned(), 
                                             
                                             new OperatorPartialConstantFolding(), 
                                         }.ToList();

        }


        public static List<OptimizationPass> BuildOptimizationPasses0()
        {
            return new OptimizationPass[]
                                         {
                                             new DeleteVregAssignedAndUsedNextLine(), 
                                             new DeleteVregAssignedVariableAndUsedNextLine(), 
                                             new DeleteVregAsLocalAssignedAndUsedPreviousLine(), 
                                             new ConstantVariablePropagation(), 
                                             new ConstantVariableOperatorPropagation(), 
                                             new ConstantVariablePropagationInCall(), 
                                             
                                             new DeleteJumpNextLine(), 
                                             new RemoveUnreferencedLabels(), 
                                             new MergeConsecutiveLabels(), 
                                             
                                             new ConstantVariableBranchOperatorPropagation(), 
                                             new OperatorConstantFolding(), 
                                             new DceVRegAssigned(),
                                         }.ToList();

        }

        public static int OptimizerLevel
        {
            get { return _optimizerLevel; }
            set
            {
                _optimizerLevel = value;
                var optimizationList = BuildOptimizationPasses0();
                if (_optimizerLevel >= 1)
                {
                    optimizationList.AddRange(BuildOptimizationPasses1());
                }
                if (_optimizerLevel >= 2)
                {
                    optimizationList.AddRange(BuildOptimizationPasses2());

                }
                OptimizationPasses = optimizationList;

            }
        }
    }
}