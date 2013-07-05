using System;
using System.Collections.Generic;
using System.Reflection;

namespace CodeRefractor.RuntimeBase.Shared
{
    public static class ReflectionUtils
    {
        public static List<FieldInfo> GetAllFields(this Type type)
        {
            var result = new List<FieldInfo>();
            result.AddRange(type.GetFields());
            result.AddRange(type.GetFields(BindingFlags.NonPublic | BindingFlags.Instance));
            return result;
        }

        public static T GetCustomAttribute<T>(this Type t) where T:Attribute
        {
            var customAttributes = t.GetCustomAttributes(typeof (T),false);
            if (customAttributes.Length == 0)
                return default(T);
            return (T) customAttributes[0];
        }

        public static T GetCustomAttribute<T>(this MemberInfo member) where T : Attribute
        {
            var customAttributes = member.GetCustomAttributes(typeof(T), false);
            if (customAttributes.Length == 0)
                return default(T);
            return (T)customAttributes[0];
        }
    }
}
