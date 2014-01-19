using System;
using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.RuntimeBase.Runtime
{
    public static class RuntimeLibraryUtils
    {
        public static Type GetReversedType(this Type type)
        {
            Type result;
            if (CrRuntimeLibrary.Instance.MappedTypes.TryGetValue(type, out result)) 
                return result;
            var newType = type.ResolveTypeByResolvers();
            if (newType != type)
            {
                CrRuntimeLibrary.Instance.MappedTypes[type] = newType;
                return newType;
            }
            return type;
        }
    }
}