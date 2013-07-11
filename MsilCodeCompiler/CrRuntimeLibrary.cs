#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.Compiler.FrontEnd;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.Compiler
{
    public class CrRuntimeLibrary
    {
        private readonly Dictionary<string, CppMethodBodyAttribute> _supportedCppMethods =
            new Dictionary<string, CppMethodBodyAttribute>();

        private readonly Dictionary<string, MetaLinker> _supportedCilMethods = new Dictionary<string, MetaLinker>();
        private readonly Dictionary<string, MethodBase> _supportedMethods = new Dictionary<string, MethodBase>();
        public readonly Dictionary<Type, Type> MappedTypes = new Dictionary<Type, Type>();
        public readonly Dictionary<Type, Type> ReverseMappedTypes = new Dictionary<Type, Type>();

        private readonly Dictionary<Type, List<CppMethodDefinition>> _crMethods =
            new Dictionary<Type, List<CppMethodDefinition>>();


        public readonly Dictionary<string, MethodBase> UsedCppMethods = new Dictionary<string, MethodBase>();
        public readonly Dictionary<string, MetaLinker> UsedCilMethods = new Dictionary<string, MetaLinker>();


        public void ScanAssembly(Assembly assembly)
        {
            foreach (var item in assembly.GetTypes())
            {
                var mapTypeAttr = item.GetCustomAttribute<MapTypeAttribute>();
                if (mapTypeAttr != null)
                {
                    MappedTypes[mapTypeAttr.MappedType] = item;
                    ReverseMappedTypes[item] = mapTypeAttr.MappedType;
                }
                ScanType(item);
            }

            foreach (var item in assembly.GetTypes())
            {
                var mapTypeAttr = item.GetCustomAttribute<MapTypeAttribute>();
                if (mapTypeAttr == null)
                    continue;
                ScanTypeForCilMethods(item);
            }
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
                linker.ComputeLabels(methodInfo);
                linker.EvaluateMethods();
                _supportedCilMethods[format] = linker;
            }
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
            if(methodNativeDescription.IsPure)
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
                cilLinkerMethod.ComputeDependencies(cilLinkerMethod.EntryPoint);
                return true;
            }
            UsedCppMethods.Add(methodDefinition, method);
            return true;
        }
    }
}