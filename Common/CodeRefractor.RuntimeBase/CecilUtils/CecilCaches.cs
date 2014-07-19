#region Uses

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Xml;

#endregion

namespace CodeRefractor.CecilUtils
{
    public static class CecilCaches
    {
        private static readonly Dictionary<string, Assembly> AsmsAssemblies = new Dictionary<string, Assembly>();
        private static readonly Dictionary<string, Type> AsmTypes = new Dictionary<string, Type>();
        public const BindingFlags AllFlags = (BindingFlags) (-1);

        public static Assembly LoadCachedAssembly(string fullName)
        {
            Assembly result;
            if (AsmsAssemblies.TryGetValue(fullName, out result))
            {
                return result;
            }
            result = Assembly.LoadFile(fullName);
            AsmsAssemblies[fullName] = result;
            return result;
        }

        static CecilCaches()
        {
            RegisterType<int>();
            RegisterType<System.Collections.ArrayList>();
            RegisterType<XmlElement>();
        }

        public static void RegisterType<T>()
        {
            var type = typeof (T);
            var typeName = type.FullName;
            AsmTypes[typeName] = type;
            LoadCachedAssembly(type.Assembly.Location);
        }
    }
}