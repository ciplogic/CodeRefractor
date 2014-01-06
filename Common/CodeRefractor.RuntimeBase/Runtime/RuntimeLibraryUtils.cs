using System;

namespace CodeRefractor.RuntimeBase.Runtime
{
    public static class RuntimeLibraryUtils
    {
        public static Type GetReversedType(this Type type)
        {
            Type result;
            if (!CrRuntimeLibrary.Instance.MappedTypes.TryGetValue(type, out result))
                return type;
            return result;
        }
    }
}