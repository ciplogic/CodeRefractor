using System;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public static class UsedTypeListUtils
    {
        public static Type ResolveTypeByResolvers(this Type type)
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
    }
}