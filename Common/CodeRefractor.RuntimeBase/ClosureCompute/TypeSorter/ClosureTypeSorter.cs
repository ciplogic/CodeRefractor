#region Uses

using System;
using System.Collections.Generic;

#endregion

namespace CodeRefractor.ClosureCompute.TypeSorter
{
    class ClosureTypeSorter
    {
        readonly ClosureEntities _crRuntime;
        readonly List<Type> _types;

        public ClosureTypeSorter(IEnumerable<Type> types, ClosureEntities crRuntime)
        {
            _types = new List<Type>(types);
            _crRuntime = crRuntime;
        }

        public List<Type> DoSort()
        {
            //Sorting types this way won't work especially when using virtuals and inheritance ..
            var types = new HashSet<Type>();
            foreach (var type in _types)
            {
                var mappedType = type.GetMappedType(_crRuntime);
                types.Add(mappedType);
            }
            var result = new List<Type>(types);
            result.Sort(new TypeComparer(_crRuntime));
            return result;
        }
    }
}