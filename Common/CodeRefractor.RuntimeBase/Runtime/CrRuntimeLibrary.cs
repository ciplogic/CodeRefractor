#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.RuntimeBase.Runtime
{
    public class CrRuntimeLibrary
    {
        private readonly Dictionary<string, MetaLinker> _supportedCilMethods = new Dictionary<string, MetaLinker>();
        private readonly Dictionary<string, MethodBase> _supportedMethods = new Dictionary<string, MethodBase>();
        public readonly Dictionary<Type, Type> MappedTypes = new Dictionary<Type, Type>();
        public readonly Dictionary<Type, MapTypeAttribute> TypeAttribute = new Dictionary<Type, MapTypeAttribute>();

        private readonly Dictionary<Type, List<CppMethodDefinition>> _crMethods =
            new Dictionary<Type, List<CppMethodDefinition>>();


        public readonly Dictionary<string, MethodBase> UsedCppMethods = new Dictionary<string, MethodBase>();
        public readonly Dictionary<string, MetaLinker> UsedCilMethods = new Dictionary<string, MetaLinker>();
        public readonly Dictionary<Type, Type> UsedTypes = new Dictionary<Type, Type>();

        private static readonly CrRuntimeLibrary StaticInstance = new CrRuntimeLibrary();
        public static CrRuntimeLibrary Instance
        {
            get { return StaticInstance; }
        }

        public static void DefaultSetup()
        {
            ProgramData.CrCrRuntimeLibrary = Instance;
        }

        public void ScanAssembly(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                var mapTypeAttr = item.GetCustomAttribute<MapTypeAttribute>();
                if (mapTypeAttr == null) continue;
                TypeAttribute[item] = mapTypeAttr; 
                MappedTypes[mapTypeAttr.MappedType] = item;
            }

        }

        private void ScanMethodFunctions(Type item)
        {
            ScanType(item);
            ScanTypeForCilMethods(item);
        }

        private void ScanTypeForCilMethods(Type item)
        {
            var methodsToScan = new List<MethodBase>();
            methodsToScan.AddRange(item.GetMethods());
            methodsToScan.AddRange(item.GetConstructors());
            foreach (var methodInfo in methodsToScan)
            {
                var methodNativeDescription = methodInfo.GetCustomAttribute<CilMethodAttribute>();
                if (methodNativeDescription == null)
                    continue;
                var format = GetMethodDescription(methodInfo);
                var linker = new MetaLinker();
                linker.SetEntryPoint(methodInfo);
                //MetaLinker.ComputeDependencies(methodInfo);
                linker.Interpret();
                _supportedCilMethods[format] = linker;
            }
        }

        public static string GetMethodDescription(MethodBase methodInfo)
        {
            var methodBase  = methodInfo.GetReversedMethod();

            return methodBase.GenerateKey();
        }


        public Type GetReverseType(Type type)
        {
            if (type == null)
                return null;

            Type result;
            if(!MappedTypes.TryGetValue(type, out result))
            {
                return null;
            }
            return result;
        }


        public Type GetMappedType(Type type)
        {
            Type result;
            if (!MappedTypes.TryGetValue(type, out result))
            {
                return null;
            }
            return result;
        }
        private List<CppMethodDefinition> GetTypesMethodList(Type type)
        {
            List<CppMethodDefinition> result;
            if (_crMethods.TryGetValue(type, out result))
                return result;
            result = new List<CppMethodDefinition>();
            _crMethods[type] = result;
            return result;
        }

        private static void ScanType(Type item)
        {
            var methods = item.GetMethods();
            foreach (var method in methods)
                ScanCppMethod(method);
        }

        private static void ScanCppMethod(MethodBase method)
        {
            var methodNativeDescription = method.GetCustomAttribute<CppMethodBodyAttribute>();
            if (methodNativeDescription == null) return;
            var reversedMethod = method.GetReversedMethod();
            var interpreter = reversedMethod.Register();
            interpreter.Kind = MethodKind.RuntimeCppMethod;
            interpreter.RuntimeLibrary.HeaderName = methodNativeDescription.Header;
            interpreter.RuntimeLibrary.Source = methodNativeDescription.Code;
            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                interpreter.RuntimeLibrary.IsPure = true;

        }

        public bool UseMethod(MethodBase methodDefinition)
        {
            var description = GetMethodDescription(methodDefinition);
            if (UsedCppMethods.ContainsKey(description)) return true;
            if (UsedCilMethods.ContainsKey(description)) return true;
            var mappedType = GetMappedType(methodDefinition.DeclaringType);
            if (mappedType == null)
                return false;
            if(!ScanForMethod(description))
                ScanMethodFunctions(mappedType);
            return ScanForMethod(description);
        }

        private bool ScanForMethod(string description)
        {
            MethodBase method;
            if (!_supportedMethods.TryGetValue(description, out method))
            {
                MetaLinker cilLinkerMethod;
                if (!_supportedCilMethods.TryGetValue(description, out cilLinkerMethod))
                    return false;
                if (UsedCilMethods.ContainsKey(description))
                    return false;
                UsedCilMethods.Add(description, cilLinkerMethod);
                MetaLinker.ComputeDependencies(cilLinkerMethod.MethodInfo);
                return true;
            }
            if (UsedCppMethods.ContainsKey(description))
                return false;
            UsedCppMethods.Add(description, method);
            return true;
        }

        public void RemapUsedTypes()
        {
            foreach (var usedCppMethod in UsedCppMethods.Values)
            {
                ScanMethodTypes(usedCppMethod);
            }
            foreach (var usedCppMethod in UsedCilMethods.Values)
            {
                ScanMethodTypes(usedCppMethod.MethodInfo);
            }

        }

        private void ScanMethodTypes(MethodBase methodInfo)
        {
            var args = methodInfo.GetParameters().ToArray();
            foreach (var arg in args)
            {
                UseClrType(arg.ParameterType);
            }
        }

        public void UseClrType(Type typeToUse)
        {
            if (!MappedTypes.ContainsKey(typeToUse))
                return;
            UsedTypes[MappedTypes[typeToUse]] = typeToUse;
        }

        public void UseType(Type type)
        {
            UseClrType(type);
        }
    }
}