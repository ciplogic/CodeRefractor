using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UsedTypeList
    {
        public readonly Dictionary<Type, TypeDescription> UserTypeDesc = new Dictionary<Type, TypeDescription>(); 
        public static TypeDescription Set(Type type)
        {
            if (type == null)
                return null;
            type = type.ResolveTypeByResolvers();
            var typeList = Instance.UserTypeDesc;
            TypeDescription typeDesc;
            var indexOf = typeList.TryGetValue(type, out typeDesc);
            if (indexOf )
            {
                return typeDesc;
            }
            var typeDescription = new TypeDescription(type);
            Instance.UserTypeDesc[type] = typeDescription;
            return typeDescription;
        }

        public static readonly UsedTypeList Instance  = new UsedTypeList();

        public static List<Type> GetUsedTypes()
        {
            return Instance.UserTypeDesc.Keys.ToList();
        }
        public static List<TypeDescription> GetDescribedTypes()
        {
            return Instance.UserTypeDesc.Values.ToList();
        }
    }
}