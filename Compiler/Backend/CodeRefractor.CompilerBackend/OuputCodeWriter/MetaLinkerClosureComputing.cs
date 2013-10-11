using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.CompilerBackend.OuputCodeWriter;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;

namespace CodeRefractor.RuntimeBase
{
    public static class MetaLinkerClosureComputing
    {
        public static List<MethodInterpreter> GetMethodClosure(MethodInterpreter entryPoints)
        {
            var result = new Dictionary<string, MethodInterpreter> { { entryPoints.ToString(), entryPoints } };
            UpdateMethodEntryClosure(entryPoints, result);
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
            
            foreach (var interpreter in toAdd)
            {
                UpdateMethodEntryClosure(interpreter, result);
            }
        }
    }
}