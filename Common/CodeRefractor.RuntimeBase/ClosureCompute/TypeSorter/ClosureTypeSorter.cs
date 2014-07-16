using System;
using System.Collections.Generic;

namespace CodeRefractor.ClosureCompute.TypeSorter
{
    class ClosureTypeSorter
    {
        private readonly HashSet<Type> _types;

        public ClosureTypeSorter(IEnumerable<Type> types)
        {
            _types = new HashSet<Type>();
            foreach (var type in types)
            {
                _types.Add(type);
            }
        }

        public List<Type> DoSort()
        {
            var result = new List<Type>(_types);
            result.Sort(new TypeComparer());
            return result;
        }
    }
}
