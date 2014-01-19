using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UsedTypeList
    {
        public Dictionary<Type, TypeDescription> _userTypeDesc = new Dictionary<Type, TypeDescription>(); 
        public static TypeDescription Set(Type type)
        {
            if (type == null)
                return null;
            type = UsedTypeListUtils.ResolveTypeByResolvers(type);
            var typeList = Instance._userTypeDesc;
            TypeDescription typeDesc;
            var indexOf = typeList.TryGetValue(type, out typeDesc);
            if (indexOf )
            {
                return typeDesc;
            }
            var typeDescription = new TypeDescription(type);
            Instance._userTypeDesc[type] = typeDescription;
            return typeDescription;
        }

        public static readonly UsedTypeList Instance  = new UsedTypeList();

        public static List<Type> GetUsedTypes()
        {
            return Instance._userTypeDesc.Keys.ToList();
        }
        public static List<TypeDescription> GetDescribedTypes()
        {
            return Instance._userTypeDesc.Values.ToList();
        }
    }
}