#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.OuputCodeWriter.BasicOperations;
using CodeRefractor.CompilerBackend.OuputCodeWriter.Platform;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MethodInterpreterCodeWriter
    {
        public static string WriteMethodCode(MethodInterpreter interpreter)
        {
            return CppMethodCodeWriter.WriteCode(interpreter);
        }
        public static string WriteMethodSignature(MethodInterpreter interpreter)
        {
            if(interpreter.Method==null)
            {
                Console.WriteLine("Should not be null");
                return "";
            }
            var sb = new StringBuilder();
            CppMethodCodeWriter.WriteSignature(interpreter.Method,sb, true);
            return sb.ToString();
        }

        internal static string WritePInvokeMethodCode(MethodInterpreter interpreter)
        {
            return interpreter.WritePlatformInvokeMethod();
        }

        internal static string WriteDelegateCallCode(MethodInterpreter interpreter)
        {
            return interpreter.WriteDelegateCallCode();
        }
        public static bool ApplyLocalOptimizations(IEnumerable<OptimizationPass> optimizationPasses, MethodInterpreter interpreter)
        {
            if (optimizationPasses == null)
                return false;
            var result = false;
            var optimizationsList = new List<OptimizationPass>(optimizationPasses);
            var didOptimize = true;

            interpreter.MidRepresentation.UpdateUseDef();
            while (didOptimize)
            {
                didOptimize = false;
                foreach (var optimizationPass in optimizationsList)
                {
                    if(!optimizationPass.CheckPreconditions(interpreter))
                        continue;
                    didOptimize = optimizationPass.Optimize(interpreter);
                    if (!didOptimize) continue;
                    interpreter.MidRepresentation.UpdateUseDef();
                    var optimizationName = optimizationPass.GetType().Name;
                    Debug.WriteLine(String.Format("Applied optimization: {0}", optimizationName));
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}