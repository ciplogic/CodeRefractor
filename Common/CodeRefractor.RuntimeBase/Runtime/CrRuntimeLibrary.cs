#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.RuntimeBase.Runtime
{
    public class CrRuntimeLibrary
    {
        private readonly Dictionary<string, CppMethodBodyAttribute> _supportedCppMethods =
            new Dictionary<string, CppMethodBodyAttribute>();

        private readonly Dictionary<string, MetaLinker> _supportedCilMethods = new Dictionary<string, MetaLinker>();
        private readonly Dictionary<string, MethodBase> _supportedMethods = new Dictionary<string, MethodBase>();
        public readonly Dictionary<Type, Type> MappedTypes = new Dictionary<Type, Type>();
        public readonly Dictionary<Type, MapTypeAttribute> TypeAttribute = new Dictionary<Type, MapTypeAttribute>();
        public readonly Dictionary<Type, Type> ReverseMappedTypes = new Dictionary<Type, Type>();

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
                ReverseMappedTypes[item] = mapTypeAttr.MappedType;
            }

        }

        private void ScanMethodFunctions(Type item)
        {
            ScanType(item);
            ScanTypeForCilMethods(item);
        }

        private void ScanTypeForCilMethods(Type item)
        {
            foreach (var methodInfo in item.GetMethods())
            {
                var methodNativeDescription = methodInfo.GetCustomAttribute<CilMethodAttribute>();
                if (methodNativeDescription == null)
                    continue;
                var format = methodInfo.GetMethodDescriptor();
                var linker = new MetaLinker();
                linker.SetEntryPoint(methodInfo);
                //MetaLinker.ComputeDependencies(methodInfo);
                linker.Interpret();
                _supportedCilMethods[format] = linker;
            }
        }

        public Type GetReverseType(Type type)
        {
            Type result;
            if(!ReverseMappedTypes.TryGetValue(type, out result))
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

        private void ScanType(Type item)
        {
            var methods = item.GetMethods();
            foreach (var method in methods)
                ScanCppMethod(item, method);
        }

        private void ScanCppMethod(Type declaringType, MethodBase method)
        {
            var methodNativeDescription = method.GetCustomAttribute<CppMethodBodyAttribute>();
            if (methodNativeDescription == null) return;
            var format = method.GetMethodDescriptor();
            _supportedCppMethods[format] = methodNativeDescription;
            _supportedMethods[format] = method;
            var methodList = GetTypesMethodList(declaringType);
            var cppMethodDefinition = new CppMethodDefinition
                                          {
                                              DeclaringType = method.DeclaringType,
                                              MappedType = declaringType,
                                              AttributeData = methodNativeDescription,
                                              MethodDefinition = method
                                          };
            var pureAttribute = method.GetCustomAttribute<PureMethodAttribute>();
            if (pureAttribute != null)
                PureMethodTable.AddPureFunction(method);
            methodList.Add(cppMethodDefinition);
        }

        public bool UseMethod(string methodDefinition)
        {
            if (UsedCppMethods.ContainsKey(methodDefinition)) return true;
            if (UsedCilMethods.ContainsKey(methodDefinition)) return true;
            MethodBase method;
            if (!_supportedMethods.TryGetValue(methodDefinition, out method))
            {
                MetaLinker cilLinkerMethod;
                if (!_supportedCilMethods.TryGetValue(methodDefinition, out cilLinkerMethod))
                    return false;
                UsedCilMethods.Add(methodDefinition, cilLinkerMethod);
                MetaLinker.ComputeDependencies(cilLinkerMethod.MethodInfo);
                return true;
            }
            UsedCppMethods.Add(methodDefinition, method);
            return true;
        }

        public void RemapUsedTypes()
        {
            foreach (var usedCppMethod in UsedCppMethods.Values)
            {
                var declaredType = usedCppMethod.DeclaringType;
                ScanMethodTypes(usedCppMethod);
                UseRuntimeType(declaredType);
            }
            foreach (var usedCppMethod in UsedCilMethods.Values)
            {
                var declaredType = usedCppMethod.MethodInfo.DeclaringType;
                ScanMethodTypes(usedCppMethod.MethodInfo);
                UseRuntimeType(declaredType);
            }

        }

        private void ScanMethodTypes(MethodBase methodInfo)
        {
            var args = methodInfo.GetParameters().ToArray();
            foreach (var arg in args)
            {
                UseClrType(arg.ParameterType);
                UseRuntimeType(arg.ParameterType);
            }
        }

        public void UseRuntimeType(Type typeToUse)
        {
            if(!ReverseMappedTypes.ContainsKey(typeToUse))
                return;
            UsedTypes[typeToUse] = ReverseMappedTypes[typeToUse];
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