#region Usings

using System;
using System.Collections.Generic;
using CodeRefractor.ClosureCompute;
using CodeRefractor.CodeWriter.BasicOperations;
using CodeRefractor.CodeWriter.Output;
using CodeRefractor.CodeWriter.Platform;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.MiddleEnd.Optimizations.Common;
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

        public static void WriteMethodSignature(CodeOutput codeOutput, MethodInterpreter interpreter, ClosureEntities closureEntities)
        {
            if (interpreter.Method == null)
            {
                Console.WriteLine("Should not be null");
                return;
            }

            CppWriteSignature.WriteSignature(codeOutput, interpreter, closureEntities, true);
        }

        internal static string WritePInvokeMethodCode(PlatformInvokeMethod interpreter, ClosureEntities crRuntime)
        {
            return interpreter.WritePlatformInvokeMethod(crRuntime);
        }

        public static string WriteDelegateCallCode(MethodInterpreter interpreter)
        {
            return interpreter.WriteDelegateCallCode();
        }

        public static bool ApplyLocalOptimizations(List<OptimizationPassBase> optimizationPasses, CilMethodInterpreter interpreter, ClosureEntities entities)
        {
            if (optimizationPasses == null)
                return false;
            if (interpreter.Method.IsAbstract)
                return false;
            var result = false;
            var optimizationsList = new List<OptimizationPassBase>(optimizationPasses);
            var areOptimizationsAvailable = true;
            while (areOptimizationsAvailable)
            {
                interpreter.MidRepresentation.UpdateUseDef();
                areOptimizationsAvailable = false;
                foreach (var optimizationPass in optimizationsList)
                {
                    var optimizationName = optimizationPass.GetType().Name;
                    if (!optimizationPass.CheckPreconditions(interpreter, entities))
                        continue;
                    areOptimizationsAvailable = optimizationPass.ApplyOptimization(interpreter, entities);

                    if (!areOptimizationsAvailable) continue;
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