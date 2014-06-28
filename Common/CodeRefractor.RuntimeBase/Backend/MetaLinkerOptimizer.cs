#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Backend.ComputeClosure;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MetaLinkerOptimizer
    {
        public static MethodInterpreter CreateLinkerFromEntryPoint(this MethodInfo definition,
            ProgramClosure programClosure)
        {
            var methodInterpreter = definition.Register();
            MetaLinker.Interpret(methodInterpreter, programClosure.Runtime);

            OptimizeMethods(LinkerInterpretersTable.Methods);
            var foundMethodCount = 1;
            var canContinue = true;
            while (canContinue)
            {
                var dependencies = methodInterpreter.GetMethodClosure(programClosure.Runtime);
                canContinue = foundMethodCount != dependencies.Count;
                foundMethodCount = dependencies.Count;
                foreach (var interpreter in dependencies)
                {
                    MetaLinker.Interpret(interpreter, programClosure.Runtime);
                }
                OptimizeMethods(LinkerInterpretersTable.Methods);
            }

            return methodInterpreter;
        }

        public static void OptimizeMethods(Dictionary<string, MethodInterpreter> methodsToOptimize)
        {
            LinkerInterpretersTable.Clear();

            ApplyOptimizations(methodsToOptimize.Values.ToList());
        }

        public static void ApplyOptimizations(List<MethodInterpreter> methodsToOptimize)
        {
            methodsToOptimize = methodsToOptimize.Where(m => m.Kind == MethodKind.Default).ToList();

            methodsToOptimize.RemoveAll(mth => mth == null);
            bool doOptimize;
            do
            {
                doOptimize = false;

                var inFunctionOptimizations = GetOptimizationByOptimizationKind(OptimizationKind.InFunction);
                var methodsArray = methodsToOptimize.ToArray();
                //Parallel.ForEach(methodsToOptimize, methodBase=> 
                foreach (var methodBase in methodsArray)
                {
                    var interpreter = methodBase;
                    Console.WriteLine("+ Optimize locally: '{0}'", methodBase.Method.Name);
                    MethodInterpreterCodeWriter.ApplyLocalOptimizations(
                        inFunctionOptimizations, interpreter);
                }
                //);
                foreach (var methodBase in methodsArray)
                {
                    var interpreter = methodBase;
                    Console.WriteLine("+ Optimize globally: '{0}'", methodBase.Method.Name);
                    doOptimize |= MethodInterpreterCodeWriter.ApplyLocalOptimizations(
                        GetOptimizationByOptimizationKind(OptimizationKind.Global), interpreter);
                }
            } while (doOptimize);
        }

        private static List<ResultingOptimizationPass> GetOptimizationByOptimizationKind(OptimizationKind optimizationKind)
        {
            List<ResultingOptimizationPass> inFunctionOptimizations = new List<ResultingOptimizationPass>();
            List<ResultingOptimizationPass> inFunction;
            if (OptimizationLevelBase.SortedOptimizations.TryGetValue(optimizationKind, out inFunction))
                inFunctionOptimizations = inFunction;
            return inFunctionOptimizations;
        }
    }
}