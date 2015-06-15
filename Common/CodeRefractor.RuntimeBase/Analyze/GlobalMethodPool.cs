#region Uses

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.Cil;
using CodeRefractor.Runtime.Annotations;

#endregion

namespace CodeRefractor.Analyze
{
    public static class GlobalMethodPool
    {
        private static readonly SortedDictionary<MethodInterpreterKey, MethodInterpreter> Interpreters =
            new SortedDictionary<MethodInterpreterKey, MethodInterpreter>();

        public static readonly Dictionary<Assembly, CrTypeResolver> TypeResolvers
            = new Dictionary<Assembly, CrTypeResolver>();

        private static readonly Dictionary<MethodBase, string> CachedKeys = new Dictionary<MethodBase, string>();

        public static void Register(MethodInterpreter interpreter)
        {
            var method = interpreter.Method;
            if (method == null)
                throw new InvalidDataException("Method is not mapped correctly");
            Interpreters[interpreter.ToKey()] = interpreter;
        }

        public static MethodInterpreter Register(this MethodBase method)
        {
            SetupTypeResolverIfNecesary(method);
            var interpreter = new CilMethodInterpreter(method);
            Register(interpreter);

            var resolved = Resolve(method);
            if (resolved != null)
            {
                return resolved;
            }
            return null;
        }

        public static MethodInterpreter Resolve(MethodBase interpreter)
        {
            SetupTypeResolverIfNecesary(interpreter);
            var resolvers = GetTypeResolvers();
            foreach (var resolver in resolvers)
            {
                var resolved = resolver.Resolve(interpreter);
                if (resolved != null)
                    return resolved;
            }
            return null;
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