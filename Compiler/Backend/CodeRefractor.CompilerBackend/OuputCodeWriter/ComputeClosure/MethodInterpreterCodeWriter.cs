#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Platform;
using CodeRefractor.CodeWriter.TypeInfoWriter;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Optimizations;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter.ComputeClosure
{
    public static class MethodInterpreterCodeWriter
    {
        public static string WriteMethodCode(MethodInterpreter interpreter, TypeDescriptionTable typeTable)
        {
            return CppMethodCodeWriter.WriteCode(interpreter, typeTable);
        }

        public static string WriteMethodSignature(MethodInterpreter interpreter)
        {
            if (interpreter.Method == null)
            {
                Console.WriteLine("Should not be null");
                return "";
            }
            var sb = CppWriteSignature.WriteSignature(interpreter, true);
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
            while (didOptimize)
            {
                interpreter.MidRepresentation.UpdateUseDef();
                didOptimize = false;
                var useDef = interpreter.MidRepresentation.UseDef;
                foreach (var optimizationPass in optimizationsList)
                {
                    var optimizationName = optimizationPass.GetType().Name;
                    if (!optimizationPass.CheckPreconditions(interpreter))
                        continue;
                    var prevCount = interpreter.MidRepresentation.LocalOperations.Count;
                    didOptimize = optimizationPass.Optimize(interpreter);

                    var actualCount = interpreter.MidRepresentation.LocalOperations.Count;
                    if (!didOptimize && actualCount != prevCount)
                    {
                        Console.WriteLine("Wrong optimization code: {0}", optimizationName);
                    }
                    if (!didOptimize) continue;
                    interpreter.MidRepresentation.UpdateUseDef();
                    //Console.WriteLine(String.Format("Applied optimization: {0}", optimizationName));
                    result = true;
                    break;
                }
            }
            return result;
        }
    }
}