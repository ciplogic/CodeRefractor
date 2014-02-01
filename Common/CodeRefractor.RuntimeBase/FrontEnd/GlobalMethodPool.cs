using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.RuntimeBase.FrontEnd
{
    public static class GlobalMethodPool
    {
        private static readonly SortedDictionary<string, MethodInterpreter>
            Interpreters = new SortedDictionary<string, MethodInterpreter>();

        public static Dictionary<Assembly, CrTypeResolver> TypeResolvers = new Dictionary<Assembly, CrTypeResolver>();  

        public static void Register(MethodInterpreter interpreter)
        {
            var method = interpreter.Method;
            var methodDefinitionKey = GenerateKey(method);
            Interpreters[methodDefinitionKey] = interpreter;
            
        }

        public static MethodInterpreter Register(this MethodBase method)
        {
            SetupTypeResolverIfNecesary(method);
           
            var interpreter = GetRegisteredInterpreter(method);
            if (interpreter != null)
                return interpreter;
            interpreter = new MethodInterpreter(method);
            Register(interpreter);
            Resolve(interpreter);
            return interpreter;
        }

        static void Resolve(MethodInterpreter interpreter)
        {
            var resolvers = GetTypeResolvers();
            foreach (var resolver in resolvers)
            {
                if(resolver.Resolve(interpreter))
                    return;
            }
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
            if (method.DeclaringType == null) return;
            var assembly = method.DeclaringType.Assembly;

            var hasValue = TypeResolvers.ContainsKey(assembly);
            if (hasValue)
                return;
            var resolverType = assembly.GetType("TypeResolver");
            
            CrTypeResolver resolver=null;
            if(resolverType!=null)
                resolver = (CrTypeResolver) Activator.CreateInstance(resolverType);
            TypeResolvers[assembly] = resolver;
        }
        static readonly Dictionary<MethodBase, string> CachedKeys = new Dictionary<MethodBase, string>(); 
        public static string GenerateKey(this MethodBase method)
        {
            string result;
            if (CachedKeys.TryGetValue(method, out result)) return result;
            result = method.WriteHeaderMethod(false);
            CachedKeys[method] = result;
            return result;
        }

        public static MethodInterpreter GetRegisteredInterpreter(this MethodBase method)
        {
            var methodDefinitionKey = GenerateKey(method);
            MethodInterpreter result;
            if (!Interpreters.TryGetValue(methodDefinitionKey, out result))
                return null;
            return result;
        }

        public static MethodBase GetReversedMethod(this MethodBase methodInfo)
        {
            var reverseType = methodInfo.DeclaringType.GetMappedType();
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
                bool found = true;
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