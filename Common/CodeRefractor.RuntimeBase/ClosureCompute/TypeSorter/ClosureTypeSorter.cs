using System;
using System.Collections.Generic;
using System.Linq;
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
            //Sorting types this way won't work especially when using virtuals and inheritance ..
            var types = new HashSet<Type>();
            foreach (var type in _types)
            {
                var mappedType = type.GetMappedType(_crRuntime);
                types.Add(mappedType);
            }
            var result = new List<Type>(types);
            result.Sort(new TypeComparer(_crRuntime));
         
            //start with base classes
            var sortedTypeData = new List<Type>();
            sortedTypeData.Add(typeof(System.Object));
            sortedTypeData.Add(typeof(System.ValueType));
            
            if (result.Contains(typeof (System.Type)))
            {
                result.Remove(typeof (System.Type));
                sortedTypeData.Add(typeof(System.Type));
            }
            if (result.Contains(typeof(System.ValueType)))
                result.Remove(typeof(System.ValueType));
            result.Remove(typeof(Object));

            var list = result.ToList();

            while (result.Count > 0)
            {
                foreach (var typeData in list)
                {
                    if (sortedTypeData.Contains(typeData)) // Prevent repeats
                        continue;

                    if (sortedTypeData.Contains(typeData.BaseType) || typeData.IsInterface)
                    {
                        sortedTypeData.Add(typeData);
                        result.Remove(typeData);
                    }
                    if (result.Count == 0)
                        break;
                }
            }

            return sortedTypeData;
        }
    }
}
