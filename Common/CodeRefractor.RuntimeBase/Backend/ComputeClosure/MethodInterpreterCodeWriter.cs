#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Platform;
using CodeRefractor.RuntimeBase.CodeWriter.BasicOperations;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;
using CodeRefractor.RuntimeBase.Runtime;
using CodeRefractor.RuntimeBase.TypeInfoWriter;

#endregion

namespace CodeRefractor.RuntimeBase.Backend.ComputeClosure
{
    public static class MethodInterpreterCodeWriter
    {
        public static string WriteMethodCode(MethodInterpreter interpreter, TypeDescriptionTable typeTable, CrRuntimeLibrary crRuntime)
        {
            return CppMethodCodeWriter.WriteCode(interpreter, typeTable, crRuntime);
        }

        public static string WriteMethodSignature(MethodInterpreter interpreter, CrRuntimeLibrary crRuntime)
        {
            if (interpreter.Method == null)
            {
                Console.WriteLine("Should not be null");
                return "";
            }
            var sb = CppWriteSignature.WriteSignature(interpreter, crRuntime, true);
            return sb.ToString();
        }

        internal static string WritePInvokeMethodCode(MethodInterpreter interpreter)
        {
            return interpreter.WritePlatformInvokeMethod();
        }

        public static string WriteDelegateCallCode(MethodInterpreter interpreter)
        {
            return interpreter.WriteDelegateCallCode();
        }

        public static bool ApplyLocalOptimizations(IEnumerable<OptimizationPass> optimizationPasses,
            MethodInterpreter interpreter)
        {
            if (optimizationPasses == null)
                return false;
            if (interpreter.Method.IsAbstract)
                return false;
            var result = false;
            var optimizationsList = new List<OptimizationPass>(optimizationPasses);
            var didOptimize = true;
            if (interpreter.Method.Name == "pollEvents")
            {
                
            }
            while (didOptimize)
            {
                interpreter.MidRepresentation.UpdateUseDef();
                didOptimize = false;
                foreach (var optimizationPass in optimizationsList)
                {
                    var optimizationName = optimizationPass.GetType().Name;
                    if (!optimizationPass.CheckPreconditions(interpreter))
                        continue;
                    didOptimize = optimizationPass.Optimize(interpreter);

                    if (!didOptimize) continue;
                    var useDef = interpreter.MidRepresentation.UseDef;
                    interpreter.MidRepresentation.UpdateUseDef();
                    Console.WriteLine(String.Format("Applied optimization: {0}", optimizationName));
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}