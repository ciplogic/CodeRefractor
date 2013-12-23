#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.CompilerBackend.OuputCodeWriter.BasicOperations;
using CodeRefractor.CompilerBackend.OuputCodeWriter.Platform;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public class MethodInterpreterCodeWriter
    {
        public MethodInterpreter Interpreter { get; set; }

        public string WriteMethodCode()
        {
            return CppMethodCodeWriter.WriteCode(Interpreter.MidRepresentation);
        }
        public string WriteMethodSignature()
        {
            if(Interpreter.MidRepresentation.Method==null)
            {
                Console.WriteLine("Should not be null");
                return "";
            }
            var sb = new StringBuilder();
            CppMethodCodeWriter.WriteSignature(Interpreter.MidRepresentation.Method,sb, true);
            return sb.ToString();
        }

        internal string WritePInvokeMethodCode()
        {
            return Interpreter.WritePlatformInvokeMethod();
        }

        internal string WriteDelegateCallCode()
        {
            return Interpreter.WriteDelegateCallCode();
        }


        public string WriteHeaderMethod()
        {
            var methodBase = Interpreter.Method;
            return methodBase.WriteHeaderMethod();
        }

        public bool ApplyLocalOptimizations(IEnumerable<OptimizationPass> optimizationPasses)
        {
            if (optimizationPasses == null)
                return false;
            var result = false;
            var optimizationsList = new List<OptimizationPass>(optimizationPasses);
            var didOptimize = true;
            while (didOptimize)
            {
                didOptimize = false;
                foreach (var optimizationPass in optimizationsList)
                {
                    if(!optimizationPass.CheckPreconditions(Interpreter.MidRepresentation))
                        continue;
                    didOptimize = optimizationPass.Optimize(Interpreter.MidRepresentation);
                    if (didOptimize)
                    {
                        var optimizationName = optimizationPass.GetType().Name;
                        Debug.WriteLine(String.Format("Applied optimization: {0}", optimizationName));
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
    }
}