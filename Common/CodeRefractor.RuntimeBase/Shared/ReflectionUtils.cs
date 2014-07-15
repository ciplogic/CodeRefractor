#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.CecilUtils;

#endregion

namespace CodeRefractor.RuntimeBase.Shared
{
    public static class ReflectionUtils
    {
        public static List<FieldInfo> GetAllFields(this Type type)
        {
            var result = new List<FieldInfo>();
            result.AddRange(type.GetFields(CecilCaches.AllFlags));
            return result;
        }

        public static T GetCustomAttribute<T>(this Type t) where T : Attribute
        {
            var customAttributes = t.GetCustomAttributes(typeof (T), false);
            if (customAttributes.Length == 0)
                return default(T);
            return (T) customAttributes[0];
        }

        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            var customAttributes = member.GetCustomAttributes(typeof (T), false);
            if (customAttributes.Length == 0)
                return default(T);
            return (T) customAttributes[0];
        }
    }
}