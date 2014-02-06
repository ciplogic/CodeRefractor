#region Usings

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeRefractor.CompilerBackend.Optimizations.Inliner;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.Config;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MetaLinkerOptimizer
    {
        public static void OptimizeMethods(Dictionary<string, MethodInterpreter> methodsToOptimize)
        {
            LinkerInterpretersTable.Clear();

            ApplyOptimizations(methodsToOptimize.Values.ToList());
        }

        public static void ApplyOptimizations(List<MethodInterpreter> methodsToOptimize)
        {
            methodsToOptimize = methodsToOptimize.Where(m => m.Kind == MethodKind.Default).ToList();
            bool doOptimize;
            do
            {
                doOptimize = false;
                var toRemove = methodsToOptimize.Where(mth => mth== null).ToArray();
                foreach (var item in toRemove)
                {
                    methodsToOptimize.Remove(item);
                }
                var inFunctionOptimizations = CommandLineParse.SortedOptimizations[OptimizationKind.InFunction];
                
                Parallel.ForEach(methodsToOptimize, methodBase=> 
                //foreach (var methodBase in methodsToOptimize)
                {
                    var interpreter = methodBase;
                    MethodInterpreterCodeWriter.ApplyLocalOptimizations(
                        inFunctionOptimizations, interpreter);
                }
                    );
                foreach (var methodBase in methodsToOptimize)
                {
                    var interpreter = methodBase;
                    doOptimize = MethodInterpreterCodeWriter.ApplyLocalOptimizations(
                        CommandLineParse.SortedOptimizations[OptimizationKind.Global], interpreter);
                }
            } while (doOptimize);

        
        }


    }
}