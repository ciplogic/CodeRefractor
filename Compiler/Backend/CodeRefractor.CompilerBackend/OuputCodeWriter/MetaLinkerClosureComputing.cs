using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.Linker;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Runtime;

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MetaLinkerClosureComputing
    {
        public static List<MethodInterpreter> GetMethodClosure(this MethodInterpreter entryPoints)
        {
            var result = new Dictionary<string, MethodInterpreter> { { entryPoints.ToString(), entryPoints } };
            UpdateMethodEntryClosure(entryPoints, result);
            return result.Values.ToList();
        }


        private static void UpdateMethodEntryClosure(MethodInterpreter entryPoint, Dictionary<string, MethodInterpreter> result)
        {
            var operations = entryPoint.MidRepresentation.LocalOperations;
            var localOperations = operations.Where(op => op.Kind == OperationKind.Call).ToArray();
            var toAdd = HandleCallInstructions(result, localOperations);
            localOperations = operations.Where(op => op.Kind == OperationKind.LoadFunction).ToArray();
            HandleLoadFunctionInstructions(result, localOperations, toAdd);

            HandleTypeInitializers(result, toAdd);
            
            HandleGenerics(result, toAdd);
        }

        private static void HandleGenerics(Dictionary<string, MethodInterpreter> result, List<MethodInterpreter> toAdd)
        {
            foreach (var interpreter in toAdd)
            {
                if (interpreter.IsGenericDeclaringType())
                {
                    //var genericSpecialization = interpreter.Clone();
                    //genericSpecialization.Specialize();
                    UpdateMethodEntryClosure(interpreter, result);
                }
                else
                    UpdateMethodEntryClosure(interpreter, result);
            }
        }

        private static void HandleTypeInitializers(Dictionary<string, MethodInterpreter> result, List<MethodInterpreter> toAdd)
        {
            foreach (var it in toAdd)
            {
                var declaringType = it.Method.DeclaringType;
                if (declaringType == null)
                    continue;
                declaringType = declaringType.GetReversedType();
                if (declaringType.TypeInitializer == null) continue;
                var info = declaringType.TypeInitializer;
                var descInfo = info.GetMethodDescriptor();

                var interpreter = info.GetInterpreter();
                result[descInfo] = interpreter;
                UpdateMethodEntryClosure(interpreter, result);
            }
        }

        private static void HandleLoadFunctionInstructions(Dictionary<string, MethodInterpreter> result, LocalOperation[] localOperations, List<MethodInterpreter> toAdd)
        {
            if(localOperations.Length==0)
                return;
            foreach (var localOperation in localOperations)
            {
                var functionPointer = (FunctionPointerStore) localOperation.Value;
                var info = functionPointer.FunctionPointer;
                var descInfo = info.GetMethodDescriptor();
                if (result.ContainsKey(descInfo))
                    continue;
                var interpreter = info.GetInterpreter();
                if (interpreter == null)
                    continue;
                result[descInfo] = interpreter;
                toAdd.Add(interpreter);
            }
        }

        private static List<MethodInterpreter> HandleCallInstructions(Dictionary<string, MethodInterpreter> result, LocalOperation[] localOperations)
        {
            var toAdd = new List<MethodInterpreter>();
            if (localOperations.Length == 0)
                return toAdd;
            foreach (var localOperation in localOperations)
            {
                var methodData = (MethodData) localOperation.Value;
                var info = methodData.Info;
                if (info.DeclaringType == typeof (object)
                    || info.DeclaringType == typeof (IntPtr))
                    continue;
                var descInfo = info.GetMethodDescriptor();
                if (result.ContainsKey(descInfo))
                    continue;
                var interpreter = info.Register();
                if (interpreter == null)
                    continue;
                var isGenericDeclaringType = interpreter.IsGenericDeclaringType();
                if (isGenericDeclaringType)
                {
                    interpreter.Specialize();
                }
                var interpreterExists = result.ContainsKey(descInfo);
                if (!interpreterExists)
                {
                    result[descInfo] = interpreter;
                    UpdateMethodEntryClosure(interpreter, result);
                }

                toAdd.Add(interpreter);
            }
            return toAdd;
        }
    }
}