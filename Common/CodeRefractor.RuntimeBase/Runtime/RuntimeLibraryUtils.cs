#region Usings

using System;
using CodeRefractor.ClosureCompute;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.Runtime
{
    public static class RuntimeLibraryUtils
    {
        public static Type GetReversedType(this Type type, ClosureEntities crRuntime)
        {
            Type result;
            if (crRuntime.MappedTypes.TryGetValue(type, out result))
                return result;
            var newType = type.ResolveTypeByResolvers();
            if (newType != type)
            {
                crRuntime.MappedTypes[type] = newType;
                return newType;
            }
            return type;
        }
    }
}