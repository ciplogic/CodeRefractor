#region Uses

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Xml;
using Mono.Cecil;

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
            result = Assembly.Load(fullName);
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
            LoadCachedAssembly(type.Assembly.FullName);
        }

        public static Type LoadCachedType(TypeReference cecilType)
        {
        	if(String.IsNullOrEmpty(cecilType.Namespace)) {
        		return LoadCachedType(cecilType.Name);
        	}
        	return LoadCachedType(String.Format("{0}.{1}", cecilType.Namespace, cecilType.Name));
        }

        public static Type LoadCachedType(string fullType)
        {
            Type result;
            if (AsmTypes.TryGetValue(fullType, out result))
            {
                return result;
            }
            foreach (var asmsAssembly in AsmsAssemblies.Values)
            {
                result = asmsAssembly.GetType(fullType);
                if(result!=null)
                    break;
            }
            if (result == null)
            {
                Debug.WriteLine("No type is found, did you miss to register the assembly of its type");
                return result;
            }
            AsmTypes[fullType] = result;
            return result;
        }
    }
}