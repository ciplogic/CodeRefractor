#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.Analyze;
using Mono.Cecil.Cil;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd
{
    public static class MethodInterpreterUtils
    {
      

        public static Type GetGenericType(Type genericType)
        {
            if (!genericType.IsGenericType)
                return genericType;
            var result = genericType.Assembly.GetType(
                string.Format("{0}.{1}",
                    genericType.Namespace, genericType.Name));
            return result;
        }

        public static MethodBase GetGenericMethod(MethodBase method, TypeDescription declaringType)
        {
            var genericParameters = new List<Type>();
            var genericDeclaringType = GetGenericType(declaringType.ClrType);
            genericParameters.AddRange(genericDeclaringType.GetGenericArguments());
            var parameters = method.GetParameters().Select(par => par.ParameterType).ToArray();

            const BindingFlags allMemberFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                                BindingFlags.Static | BindingFlags.Instance;
            var result = method.IsConstructor
                ? (MethodBase) genericDeclaringType.GetConstructor(parameters)
                : genericDeclaringType.GetMethod(method.Name, parameters);
            if (result == null)
            {
                try
                {
                    result = genericDeclaringType.GetMethod(method.Name, allMemberFlags);
                }
                catch
                {
                }
            }

            if (result == null)
            {
                var methods = genericDeclaringType.GetMethods();
                foreach (var info in methods)
                {
                    if (method.Name == info.Name)
                    {
                        result = info;
                        break;
                    }
                }
            }
            if (result == null)
            {
            }
            return result;
        }
    }
}