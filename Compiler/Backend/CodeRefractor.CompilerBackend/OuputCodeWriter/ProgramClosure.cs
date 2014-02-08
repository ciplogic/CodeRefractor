using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeRefactor.OpenRuntime;
using CodeRefractor.CompilerBackend.Optimizations.Common;
using CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public class ProgramClosure
    {
        public List<Type> UsedTypes;
        public MethodInterpreter EntryInterpreter { get; set; }
        public List<MethodInterpreter> MethodClosure = new List<MethodInterpreter>();

        public ProgramClosure(MethodInfo entryMethod)
        {
            EntryInterpreter = entryMethod.Register();

            BuildMethodClosure();
            MetaLinkerOptimizer.ApplyOptimizations(MethodClosure);
            BuildMethodClosure();
            UsedTypes = TypesClosureLinker.GetTypesClosure(MethodClosure);
            UsedTypes.Add(typeof(CrString));
        }

        private void BuildMethodClosure()
        {
            MethodClosure.Clear();
            MethodClosure.Add(EntryInterpreter);
            MetaLinker.Interpret(EntryInterpreter);

            var foundMethodCount = 1;
            bool canContinue = true;
            while (canContinue)
            {
                var dependencies = EntryInterpreter.GetMethodClosure();
                canContinue = foundMethodCount != dependencies.Count;
                foundMethodCount = dependencies.Count;
                foreach (var interpreter in dependencies)
                {
                    MetaLinker.Interpret(interpreter);
                }
                MethodClosure = dependencies;
            }
        }

        public static void ComputeEscapeAnalysis(List<MethodInterpreter> methodClosures)
        {
            var escapeParameters = new AnalyzeParametersAreEscaping();
            var loweringVars = new InFunctionLoweringVars();
            var opSteps = new ResultingOptimizationPass[] {escapeParameters, loweringVars};
            for (var i = 0; i < 2; i++)
            {
                foreach (var pass in opSteps)
                {
                    foreach (var methodInterpreter in methodClosures)
                    {
                        pass.Optimize(methodInterpreter);
                    }
                }
            }
        }

        public StringBuilder BuildFullSourceCode()
        {
            ComputeEscapeAnalysis(MethodClosure);

            return CppCodeGenerator.GenerateSourceStringBuilder(EntryInterpreter, UsedTypes, MethodClosure);
        }
    }
}