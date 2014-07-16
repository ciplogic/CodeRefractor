using System;
using System.Collections.Generic;
using CodeRefractor.CecilUtils;

namespace CodeRefractor.ClosureCompute.TypeSorter
{
    internal class TypeComparer : IComparer<Type>
    {
        static HashSet<Type> DependencyTypes(Type type)
        {
            var result = new HashSet<Type>();
            var members = type.GetMembers(CecilCaches.AllFlags);
            foreach (var member in members)
            {
                result.Add(member.ReflectedType);
            }
            return result;
        } 
        static bool IsSmallerThan(Type x, Type y)
        {
            if (y.IsSubclassOf(x))
                return true;
            var dependantTypeOfY = DependencyTypes(y);
            return dependantTypeOfY.Contains(x);
        }
        public int Compare(Type x, Type y)
        {
            if (x == y)
                return 0;
            if(IsSmallerThan(x, y))
                return -1;
            if(IsSmallerThan(y, x))
                return 1;
            return 0;
        }
    }
}