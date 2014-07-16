using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Util;
using Mono.Cecil;

namespace CodeRefractor.CecilUtils
{
    public static class CecilWrapper
    {
        public static string GetClrName(this string name)
        {
            return name.Replace("/", "+").Replace("/", ".");
        }

        public static string GetCecilName(this string name)
        {
            return name.Replace("+", "/");
        }


        public static Type GetMappedClrType(TypeReference declaringType)
        {
            var genericType = declaringType as GenericInstanceType;
            if (genericType != null)
            {
                var genericParameters = genericType.GenericArguments;
                var clrGenericParameters = new List<Type>();
                foreach (var genParameter in genericParameters)
                {
                    GetMappedAssembly(genParameter.Module);
                    var mappedClrType = GetMappedClrType(genParameter);
                    clrGenericParameters.Add(mappedClrType);
                }
                var getGenericType = CecilCaches.LoadCachedType(genericType);
                if(clrGenericParameters.Contains(null))
                {
                	return getGenericType;
                }
                var specializedResult = getGenericType.MakeGenericType(clrGenericParameters.ToArray());
                return specializedResult;
            }
            GetMappedAssembly(declaringType.Module);
            var type = CecilCaches.LoadCachedType(declaringType.FullName);
            return type;
        }

        private static Assembly GetMappedAssembly(ModuleDefinition module)
        {
            var assembly = CecilCaches.LoadCachedAssembly(module.FullyQualifiedName);
            return assembly;
        }

        public static MethodBase GetMethod(this MethodReference definition)
        {
            GetMappedAssembly(definition.Module);

            var declaringType = definition.DeclaringType;
            var type = GetMappedClrType(declaringType);
            var methodsWithName = type.GetMembers().Where(m => m.Name == definition.Name).ToArray();
            if (methodsWithName.Length == 1 && methodsWithName[0] is MethodBase)
            {
                return (MethodBase) methodsWithName[0];
            }
            var parList = GetParameterList(definition);

            var result = GetMethodBasedOnParametersAndName(type, definition.Name, parList);
            return result;
        }

        private static Type[] GetParameterList(MethodReference definition)
        {
            var parameters = definition.Parameters;
            var parList = new List<Type>();
            foreach (var parameterDefinition in parameters)
            {
                var mappedClrType = GetMappedClrType(parameterDefinition.ParameterType);
                parList.Add(mappedClrType);
            }
            return parList.ToArray();
        }

        private static MethodBase GetMethodBasedOnParametersAndName(Type declaringType, string methodName, Type[] parList)
        {
            var methodResult = declaringType.GetMember(methodName, CecilCaches.AllFlags);
            if (methodResult.Length == 1)
                return (MethodBase) methodResult[0];
            var targetParameters = parList;

            foreach (var memberInfo in methodResult)
            {
                var methodBase = memberInfo as MethodBase;
                if (methodBase == null)
                    continue;
                var methodParameters = methodBase.GetParameters();
                if (targetParameters.Length != methodParameters.Length)
                    continue;
                var found = true;
                for (int index = 0; index < methodParameters.Length; index++)
                {
                    var methodParameter = methodParameters[index];
                    if (methodParameter.ParameterType != targetParameters[index])
                    {
                        found = false;
                        break;
                    }
                }
                if (found)
                    return methodBase;
            }
            return null;
        }


        public static bool MethodMatches(this MethodReference otherDefinition, MethodBase method)
        {
            if ((method.GetMethodName() != otherDefinition.Name) && (otherDefinition.Name != method.Name))
                return false;

            var arguments = method.GetParameters().Select(par => par.ParameterType).ToArray();

            if (arguments.Length != otherDefinition.Parameters.Count)
                return false;

            for (var index = 0; index < arguments.Length; index++)
            {
                Type argument = arguments[index];
                ParameterDefinition parameter = otherDefinition.Parameters[index];
                if (argument.FullName != parameter.ParameterType.FullName)
                    return false;
            }

            return true;
        }

     
    }
}