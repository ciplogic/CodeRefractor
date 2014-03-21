using System;
using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.RuntimeBase.Runtime
{
    public static class RuntimeLibraryUtils
    {
        public static Type GetReversedType(this Type type, CrRuntimeLibrary crRuntime)
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