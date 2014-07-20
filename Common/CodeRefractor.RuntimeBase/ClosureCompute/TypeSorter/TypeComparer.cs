using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.ClosureCompute.TypeSorter
{
    internal class TypeComparer : IComparer<Type>
    {
        private readonly ClosureEntities _crRuntime;

        public TypeComparer(ClosureEntities crRuntime)
        {
            _crRuntime = crRuntime;
        }

        HashSet<Type> DependencyTypes(Type type)
        {
            var result = new HashSet<Type>();
            var members = type.GetMembers(ClosureEntitiesBuilder.AllFlags);
            foreach (var member in members)
            {
                result.Add(member.ReflectedType.GetMappedType(_crRuntime));
            }
            return result;
        } 
        bool IsSmallerThan(Type x, Type y)
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

            x = x.GetMappedType(_crRuntime);
            y = y.GetMappedType(_crRuntime);
            if(IsSmallerThan(x, y))
                return -1;
            if(IsSmallerThan(y, x))
                return 1;
            return 0;
        }
    }
}