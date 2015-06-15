#region Uses

using System;
using System.Linq;
using System.Reflection;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public static class MethodBaseUtils
    {
        public static bool IsPinvoke(this MethodBase method)
        {
            return method.Attributes.HasFlag(MethodAttributes.PinvokeImpl);
        }

        public static MethodBaseKey ToMethodBaseKey(this MethodBase method)
        {
            var result = new MethodBaseKey(method);
            return result;
        }

        public static Type ReduceType(this Type type)
        {
            if (type.IsArray)
            {
                return ReduceType(type.GetElementType());
            }
            if (type.IsByRef)
            {
                return ReduceType(type.GetElementType());
            }
            return type;
        }

        public static bool MethodMatches(this MethodBase otherDefinition, MethodBase method)
        {
            var declaringType = method.DeclaringType;
            var otherDeclaringType = method.DeclaringType;
            if (declaringType != otherDeclaringType)
                return false;

            if ((method.GetMethodName() != otherDefinition.Name) && (otherDefinition.Name != method.Name))
                return false;


            if (method.GetReturnType().FullName != otherDefinition.GetReturnType().FullName)
                return false;
            var arguments = method.GetParameters().Select(par => par.ParameterType).ToArray();

            if (arguments.Length != otherDefinition.GetParameters().Length)
                return false;

            for (var index = 0; index < arguments.Length; index++)
            {
                var argument = arguments[index];
                var parameter = otherDefinition.GetParameters()[index];
                if (argument.FullName != parameter.ParameterType.FullName)
                    return false;
            }

            return true;
        }
    }
}