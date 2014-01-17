using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase.FrontEnd;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class UsedTypeList
    {
        public List<Type> _userTypes = new List<Type>();
        public List<TypeDescription> _userTypeDesc = new List<TypeDescription>(); 
        public static TypeDescription Set(Type type)
        {
            var typeList = Instance._userTypes;
            var indexOf = typeList.IndexOf(type);
            if (indexOf != -1)
            {
                return Instance._userTypeDesc[indexOf];
            }
            type = ResolveTypeByResolvers(type);
            var typeDescription = new TypeDescription(type);
            Instance._userTypeDesc.Add(typeDescription);
            Instance._userTypes.Add(type);
            return typeDescription;
        }

        public static Type ResolveTypeByResolvers(Type type)
        {
            var resolvers = GlobalMethodPool.GetTypeResolvers();
            foreach (var resolver in resolvers)
            {
                var newType = resolver.ResolveType(type);
                if (newType != null)
                {
                    type = newType;
                    break;
                }
            }
            return type;
        }

        public static readonly UsedTypeList Instance  = new UsedTypeList();

        public static List<Type> GetUsedTypes()
        {
            return Instance._userTypes;
        }
        public static List<TypeDescription> GetDescribedTypes()
        {
            return Instance._userTypeDesc;
        }
    }
}