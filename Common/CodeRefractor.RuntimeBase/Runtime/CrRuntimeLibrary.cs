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
    public class CrRuntimeLibrary{
        public readonly Dictionary<Type, Type> MappedTypes = new Dictionary<Type, Type>();

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
                MappedTypes[mapTypeAttr.MappedType] = item;
                ScanMethodFunctions(item);
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
            }
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

        private static void ScanType(Type item)
        {
            var methods = item.GetMethods();
            foreach (var method in methods)
                ScanCppMethod(method);
        }


    }
}