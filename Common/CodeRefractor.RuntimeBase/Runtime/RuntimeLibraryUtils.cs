using System;
using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.RuntimeBase.Runtime
{
    public static class RuntimeLibraryUtils
    {
        public static Type GetReversedType(this Type type)
        {
            Type result;
            var newType = UsedTypeList.ResolveTypeByResolvers(type);
            if (newType != type)
                return newType;
            if (!CrRuntimeLibrary.Instance.MappedTypes.TryGetValue(type, out result))
                return type;
            return result;
        }
    }
}