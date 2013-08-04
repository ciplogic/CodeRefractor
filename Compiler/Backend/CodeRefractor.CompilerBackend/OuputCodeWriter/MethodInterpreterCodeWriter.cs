#region Usings

using System.Collections.Generic;
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
            if (Interpreter.Kind == MethodKind.PlatformInvoke)
            {
                var pInvokeText = CppMethodCodeWriter.WritePlatformInvokeMethod(Interpreter);
                return pInvokeText;
            }
            return CppMethodCodeWriter.WriteCode(Interpreter.MidRepresentation);
        }

        public string WriteHeaderMethod()
        {
            var methodBase = Interpreter.Method;
            return methodBase.WriteHeaderMethod();
        }

        public void ApplyLocalOptimizations(IEnumerable<OptimizationPass> optimizationPasses)
        {
            if (optimizationPasses == null)
                return;
            var optimizationsList = new List<OptimizationPass>(optimizationPasses);
            var didOptimize = true;
            while (didOptimize)
            {
                didOptimize = false;
                foreach (var optimizationPass in optimizationsList)
                {
                    didOptimize = optimizationPass.Optimize(Interpreter.MidRepresentation);
                    if (didOptimize)
                        break;
                }
            }
        }
    }
}