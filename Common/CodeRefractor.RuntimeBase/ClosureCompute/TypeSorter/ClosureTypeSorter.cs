using System;
using System.Collections.Generic;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.ClosureCompute.TypeSorter
{
    class ClosureTypeSorter
    {
        private readonly List<Type> _types;
        private readonly ClosureEntities _crRuntime;

        public ClosureTypeSorter(IEnumerable<Type> types, ClosureEntities crRuntime)
        {
            _types = new List<Type>(types);
            _crRuntime = crRuntime;
        }

        public List<Type> DoSort()
        {
            var types = new HashSet<Type>();
            foreach (var type in _types)
            {
                var mappedType = type.GetMappedType(_crRuntime);
                types.Add(mappedType);
            }
            var result = new List<Type>(_types);
            result.Sort(new TypeComparer());
            return result;
        }
    }
}
