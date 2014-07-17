#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Platform;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Optimizations.Common;
using CodeRefractor.RuntimeBase.CodeWriter.BasicOperations;
using CodeRefractor.RuntimeBase.TypeInfoWriter;

#endregion

namespace CodeRefractor.Backend.ComputeClosure
{
    public static class MethodInterpreterCodeWriter
    {
        public static string WriteMethodCode(CilMethodInterpreter interpreter, TypeDescriptionTable typeTable, ClosureEntities closureEntities)
        {
            return CppMethodCodeWriter.WriteCode(interpreter, typeTable, closureEntities);
        }

        public static string WriteMethodSignature(MethodInterpreter interpreter, ClosureEntities closureEntities)
        {
            if (interpreter.Method == null)
            {
                Console.WriteLine("Should not be null");
                return "";
            }
            var sb = CppWriteSignature.WriteSignature(interpreter,closureEntities, true);
            return sb.ToString();
        }

        internal static string WritePInvokeMethodCode(PlatformInvokeMethod interpreter, ClosureEntities crRuntime)
        {
            return interpreter.WritePlatformInvokeMethod(crRuntime);
        }

        public static string WriteDelegateCallCode(MethodInterpreter interpreter)
        {
            return interpreter.WriteDelegateCallCode();
        }

        public static bool ApplyLocalOptimizations(IEnumerable<ResultingOptimizationPass> optimizationPasses,
            CilMethodInterpreter interpreter)
        {
            if (optimizationPasses == null)
                return false;
            if (interpreter.Method.IsAbstract)
                return false;
            var result = false;
            var optimizationsList = new List<ResultingOptimizationPass>(optimizationPasses);
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
                    Console.WriteLine("Applied optimization: {0}", optimizationName);
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}