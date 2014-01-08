#region Usings

using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MetaLinkerOptimizer
    {
        public static void OptimizeMethods(bool doInline = false)
        {
            LinkerInterpretersTable.Clear();

            
            var methodsToOptimize = LinkerInterpretersTable.Methods;
            ApplyOptimizations(doInline, methodsToOptimize.Values.ToList());
        }

        public static void ApplyOptimizations(bool doInline, List<MethodInterpreter> methodsToOptimize)
        {
            bool doOptimize;
            do
            {
                doOptimize = false;
                var toRemove = methodsToOptimize.Where(mth => mth== null).ToArray();
                foreach (var item in toRemove)
                {
                    methodsToOptimize.Remove(item);
                }
                foreach (var methodBase in methodsToOptimize)
                {
                    var interpreter = methodBase;
                    var codeWriter = new MethodInterpreterCodeWriter
                        {
                            Interpreter = interpreter
                        };
                    doOptimize = codeWriter.ApplyLocalOptimizations(
                        CommandLineParse.SortedOptimizations[OptimizationKind.InFunction]);
                }
                foreach (var methodBase in methodsToOptimize)
                {
                    var interpreter = methodBase;
                    var codeWriter = new MethodInterpreterCodeWriter
                        {
                            Interpreter = interpreter
                        };

                    doOptimize = codeWriter.ApplyLocalOptimizations(
                        CommandLineParse.SortedOptimizations[OptimizationKind.Global]);
                }
            } while (doOptimize);

        
        }


    }
}