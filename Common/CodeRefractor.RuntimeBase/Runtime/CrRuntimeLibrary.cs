#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.Runtime
{
    public class CrRuntimeLibrary
    {
        public readonly Dictionary<Type, Type> MappedTypes = new Dictionary<Type, Type>();

        public readonly Dictionary<MethodInterpreterKey, MethodInterpreter> SupportedMethods =
            new Dictionary<MethodInterpreterKey, MethodInterpreter>();

        public void ScanAssembly(Assembly assembly, ClosureEntities crRuntime)
        {
            foreach (var item in assembly.GetTypes())
            {
                var mapTypeAttr = item.GetCustomAttribute<MapTypeAttribute>();
                if (mapTypeAttr == null) continue;
                MappedTypes[mapTypeAttr.MappedType] = item;
            }
            foreach (var item in MappedTypes)
            {
                ScanMethodFunctions(item.Value, item.Key, crRuntime);
            }
        }

        private void ScanMethodFunctions(Type item, Type mappedType, ClosureEntities crRuntime)
        {
            ScanType(item, mappedType, crRuntime);
            ScanTypeForCilMethods(item, mappedType);
        }

        private void ScanTypeForCilMethods(Type item, Type mappedType)
        {
            var methodsToScan = new List<MethodBase>();
            methodsToScan.AddRange(item.GetMethods());
            methodsToScan.AddRange(item.GetConstructors());
            foreach (var methodInfo in methodsToScan)
            {
                var interpreter = methodInfo.Register();
                var iKey = interpreter.ToKey(item);
                iKey.MapTypes(MappedTypes);

                SupportedMethods[iKey] = interpreter;
                MetaLinker.Interpret(interpreter, this);
                var dependencies = MetaLinker.ComputeDependencies(methodInfo);
                foreach (var dependency in dependencies)
                {
                    MetaLinker.Interpret(dependency, this);
                }
            }
        }

        private void ScanCppMethod(MethodBase method, Type mappedType, ClosureEntities crRuntime)
        {
            var methodNativeDescription = method.GetCustomAttribute<CppMethodBodyAttribute>();
            if (methodNativeDescription == null) return;
            var reversedMethod = method.GetReversedMethod(crRuntime);
            var interpreter = reversedMethod.Register();
            interpreter.Kind = MethodKind.RuntimeCppMethod;
            var cppRepresentation = interpreter.CppRepresentation;
            cppRepresentation.Kind = CppKinds.RuntimeLibrary;
            cppRepresentation.Header = methodNativeDescription.Header;
            cppRepresentation.Source = methodNativeDescription.Code;
            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                interpreter.AnalyzeProperties.IsPure = true;
            var methodInterpreterKey = interpreter.ToKey();
            methodInterpreterKey.DeclaringType = mappedType;
            SupportedMethods[methodInterpreterKey] = interpreter;
        }

        public Type GetReverseType(Type type)
        {
            if (type == null)
                return null;

            foreach (var mappedType in MappedTypes)
            {
                if (mappedType.Value == type)
                    return mappedType.Key;
            }
            return null;
        }


        public Type GetMappedType(Type type)
        {
            Type result;
            if (!MappedTypes.TryGetValue(type, out result))
            {
                return type;
            }
            return result;
        }

        private void ScanType(Type item, Type mappedType, ClosureEntities crRuntime)
        {
            var methods = item.GetMethods();
            foreach (var method in methods)
                ScanCppMethod(method, mappedType, crRuntime);
        }

        public MethodBase GetMappedMethodInfo(MethodBase methodInfo)
        {
            var mappedDeclaredType = GetMappedType(methodInfo.DeclaringType);
            var methodName = methodInfo.Name;
            var arguments = methodInfo.GetParameters().Select(par => par.ParameterType).ToArray();
            return mappedDeclaredType.GetMethod(methodName, arguments);
        }

        public ConstructorInfo GetMappedConstructor(MethodBase constructorInfo)
        {
            var mappedDeclaredType = GetMappedType(constructorInfo.DeclaringType);
            var arguments = constructorInfo.GetParameters().Select(par => par.ParameterType).ToArray();
            return mappedDeclaredType.GetConstructor(arguments);
        }

        public bool ResolveInterpreter(MethodInterpreterKey interpreter, ref MethodInterpreter finalInterpreter)
        {
            MethodInterpreter result;
            if (SupportedMethods.TryGetValue(interpreter, out result))
            {
                finalInterpreter = result;
                return true;
            }
            return false;
        }
    }
}