#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd;
using CodeRefractor.Runtime;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase.MiddleEnd;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public static class GlobalMethodPool
    {
        private static readonly SortedDictionary<MethodInterpreterKey, MethodInterpreter> Interpreters =
            new SortedDictionary<MethodInterpreterKey, MethodInterpreter>();

        public static readonly Dictionary<Assembly, CrTypeResolver> TypeResolvers
            = new Dictionary<Assembly, CrTypeResolver>();

        public static void Register(MethodInterpreter interpreter)
        {
            var method = interpreter.Method;
            if (method == null)
                throw new InvalidDataException("Method is not mapped correctly");
            Interpreters[interpreter.ToKey()] = interpreter;
        }

        public static MethodInterpreter Register(this MethodBase method, CrRuntimeLibrary crRuntime = null)
        {
            SetupTypeResolverIfNecesary(method);
            var interpreter = new MethodInterpreter(method);
            Register(interpreter);

            if (Resolve(interpreter))
            {
                return interpreter;
            }
            if (crRuntime != null)
            {
                var methodKind = crRuntime.ResolveInterpreter(interpreter.ToKey(), ref interpreter);
                //interpreter.Kind = methodKind;
            }
            return interpreter;
        }

        public static bool Resolve(MethodInterpreter interpreter)
        {
            SetupTypeResolverIfNecesary(interpreter.Method);
            var resolvers = GetTypeResolvers();
            foreach (var resolver in resolvers)
            {
                if (resolver.Resolve(interpreter))
                    return true;
            }
            return false;
        }

        public static CrTypeResolver[] GetTypeResolvers()
        {
            var resolvers = TypeResolvers.Values
                .Where(r => r != null)
                .ToArray();
            return resolvers;
        }

        private static void SetupTypeResolverIfNecesary(MethodBase method)
        {
            try
            {

            
            if (method.DeclaringType == null) return;
            }
            catch (Exception ex)
            {

                
            }
            var assembly = method.DeclaringType.Assembly;

            var hasValue = TypeResolvers.ContainsKey(assembly);
            if (hasValue)
                return;
            var resolverType = assembly.GetTypes().FirstOrDefault(t => t.Name == "TypeResolver");

            CrTypeResolver resolver = null;
            if (resolverType != null)
                resolver = (CrTypeResolver) Activator.CreateInstance(resolverType);
            TypeResolvers[assembly] = resolver;
        }

        private static readonly Dictionary<MethodBase, string> CachedKeys = new Dictionary<MethodBase, string>();

        public static string GenerateKey(this MethodBase method, ClosureEntities crRuntime)
        {
            string result;
            if (CachedKeys.TryGetValue(method, out result)) return result;
            result = method.WriteHeaderMethod(crRuntime, writeEndColon: false);
            CachedKeys[method] = result;
            return result;
        }

        public static MethodBase GetReversedMethod(this MethodBase methodInfo, ClosureEntities crRuntime)
        {
            var reverseType = methodInfo.DeclaringType.GetMappedType(crRuntime);
            if (reverseType == methodInfo.DeclaringType)
                return methodInfo;
            var originalParameters = methodInfo.GetParameters();
            var memberInfos = reverseType.GetMember(methodInfo.Name);

            foreach (var memberInfo in memberInfos)
            {
                var methodBase = memberInfo as MethodBase;
                if (methodBase == null)
                    continue;
                var parameters = methodBase.GetParameters();
                if (parameters.Length != originalParameters.Length)
                    continue;
                var found = true;
                for (var index = 0; index < parameters.Length; index++)
                {
                    var parameter = parameters[index];
                    var originalParameter = originalParameters[index];
                    if (parameter.ParameterType == originalParameter.ParameterType) continue;
                    found = false;
                    break;
                }
                if (found)
                {
                    return methodBase;
                }
            }
            return methodInfo;
        }
    }
}