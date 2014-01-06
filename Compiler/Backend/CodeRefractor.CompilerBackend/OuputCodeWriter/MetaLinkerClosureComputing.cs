using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.Runtime;

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class MetaLinkerClosureComputing
    {
        public static List<MethodInterpreter> GetMethodClosure(MethodInterpreter entryPoints)
        {
            var result = new Dictionary<string, MethodInterpreter> { { entryPoints.ToString(), entryPoints } };
            UpdateMethodEntryClosure(entryPoints, result);
            foreach (var interpreter in result)
            {
                var declaringType = interpreter.Value.Method.DeclaringType;
                if (declaringType == null)
                    continue;

            }
            foreach (var interpreter in result)
            {
                var isGenericDeclaringType = interpreter.Value.IsGenericDeclaringType();
                if(!isGenericDeclaringType)
                    continue;
                
            }
            return result.Values.ToList();
        }


        public static void UpdateMethodEntryClosure(MethodInterpreter entryPoint, Dictionary<string, MethodInterpreter> result)
        {
            var operations = entryPoint.MidRepresentation.LocalOperations;
            var localOperations = operations.Where(op => op.Kind == OperationKind.Call).ToArray();
            var toAdd = new List<MethodInterpreter>();
            foreach (var localOperation in localOperations)
            {
                var methodData = (MethodData)localOperation.Value;
                var info = methodData.Info;
                if (info.DeclaringType == typeof(object)
                    || info.DeclaringType == typeof(IntPtr))
                    continue;
                var descInfo = info.GetMethodDescriptor();
                if (result.ContainsKey(descInfo))
                    continue;
                var interpreter = ClassTypeData.GetInterpreterStatic(info);
                if (interpreter == null)
                    continue;
                var isGenericDeclaringType = interpreter.IsGenericDeclaringType();
                if (isGenericDeclaringType)
                {
                    interpreter.Specialize();
                }
                result[descInfo] = interpreter;

                toAdd.Add(interpreter);
            }
            localOperations = operations.Where(op => op.Kind == OperationKind.LoadFunction).ToArray();
            foreach (var localOperation in localOperations)
            {
                var functionPointer = (FunctionPointerStore)localOperation.Value;
                var info = functionPointer.FunctionPointer;
                var descInfo = info.GetMethodDescriptor();
                if (result.ContainsKey(descInfo))
                    continue;
                var interpreter = ClassTypeData.GetInterpreterStatic(info);
                if (interpreter == null)
                    continue;
                result[descInfo] = interpreter;
                toAdd.Add(interpreter);
            }

            foreach (var it in toAdd)
            {
                var declaringType = it.Method.DeclaringType;
                if (declaringType == null)
                    continue;
                declaringType = declaringType.GetReversedType();
                if (declaringType.TypeInitializer == null) continue;
                var info = declaringType.TypeInitializer;
                var descInfo = info.GetMethodDescriptor();

                var interpreter = ClassTypeData.GetInterpreterStatic(info);
                result[descInfo] = interpreter;
                UpdateMethodEntryClosure(interpreter, result);
            }
            
            foreach (var interpreter in toAdd)
            {
                if (interpreter.IsGenericDeclaringType())
                {
                    var genericSpecialization = interpreter.Clone();
                    genericSpecialization.Specialize();
                    UpdateMethodEntryClosure(genericSpecialization, result);
                }else
                UpdateMethodEntryClosure(interpreter, result);
            }
        }
    }
}