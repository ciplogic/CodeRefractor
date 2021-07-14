﻿using System;
using System.IO;
using System.Reflection;
using CodeRefactor.OpenRuntime;
using CodeRefractor.ClosureCompute;
using CodeRefractor.Config;
using CodeRefractor.Util;
using static System.String;

namespace CodeRefractor.Compiler
{
    /**
     * The main application for the compiler.
     */
    public class Program
    {
        private readonly CommandLineParse _commandLineParse;
        private readonly ClosureEntitiesUtils _getClosureEntitiesUtils;

        public Program(CommandLineParse commandLineParse,
                       ClosureEntitiesUtils  getClosureEntitiesUtils)
        {
            _commandLineParse = commandLineParse;
            _getClosureEntitiesUtils = getClosureEntitiesUtils;
        }
 
        /**
         *  Parses the command line, loads the assembly, builds a closure of all the items
         *  to be built, and transforms the into source code, and writes an output C++
         *  program.
         *  @return Full path to the written CPP file.
         */
        public string CallCompiler(string inputAssemblyName)
        {
            var commandLineParse = _commandLineParse;

            if (!IsNullOrEmpty(inputAssemblyName))
            {
                commandLineParse.ApplicationInputAssembly = inputAssemblyName;
            }

            var dir = Directory.GetCurrentDirectory();
            inputAssemblyName = Path.Combine(dir, commandLineParse.ApplicationInputAssembly);
            
            var asm = Assembly.LoadFile(inputAssemblyName);
            var definition = asm.EntryPoint; // TODO: what if this is not an application, but a library without an entry point?
            var start = Environment.TickCount;

            var closureEntities = _getClosureEntitiesUtils
                .BuildClosureEntities(definition, typeof(CrString).Assembly);

            var fullSourceCode = closureEntities.BuildFullSourceCode();
            var compilationTime = Environment.TickCount - start;
            Console.WriteLine("Compilation time: {0} ms", compilationTime);

            var fullPath = commandLineParse.OutputCpp.GetFullFileName();
            fullSourceCode.Src.ToFile(fullPath);
            fullSourceCode.Header.ToFile(commandLineParse.OutputH.GetFullFileName());
            Console.WriteLine("Wrote output CPP file '{0}'.", fullPath);
            return fullPath;

            // TODO: this should be a flag, not necessarily do it automatically.
            //NativeCompilationUtils.CompileAppToNativeExe(commandLineParse.OutputCpp,
            //                                             commandLineParse.ApplicationNativeExe);
        }
 
    }
}