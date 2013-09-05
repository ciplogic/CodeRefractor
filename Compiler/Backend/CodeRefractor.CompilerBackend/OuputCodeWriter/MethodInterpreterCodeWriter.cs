#region Usings

using System.Collections.Generic;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Runtime;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public class MethodInterpreterCodeWriter
    {
        public MethodInterpreter Interpreter { get; set; }

        public string WriteMethodCode(CrRuntimeLibrary crCrRuntimeLibrary)
        {
            return CppMethodCodeWriter.WriteCode(Interpreter.MidRepresentation, crCrRuntimeLibrary);
        }
        public string WriteMethodSignature(CrRuntimeLibrary crCrRuntimeLibrary)
        {
            var sb = new StringBuilder();
            CppMethodCodeWriter.WriteSignature(Interpreter.MidRepresentation.Method,sb, crCrRuntimeLibrary, true);
            return sb.ToString();
        }

        internal string WritePInvokeMethodCode()
        {
            return Interpreter.WritePlatformInvokeMethod();
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
                        result = true;
                        break;
                    }
                }
            }
            return result;
        }
    }
}