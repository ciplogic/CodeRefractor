using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.CodeWriter.TypeInfoWriter
{
    public class TypeDescriptionTable : IComparer<Type>
    {
        private readonly List<Type> _typeClosure;

        Dictionary<Type, HashSet<Type>> _dictionary = new Dictionary<Type, HashSet<Type>>(); 

        Dictionary<Type, int> _result = new Dictionary<Type, int>(); 

        public TypeDescriptionTable(List<Type> typeClosure)
        {
            _typeClosure = typeClosure.Where(
                FilterType)
                .ToList();
        }

        private static bool FilterType(Type type)
        {
            if (type.IsValueType)
                return false;
            var isStatic = (type.IsAbstract && type.IsSealed);
            return !isStatic;
        }

        private void BuildDependantTypeDictionary()
        {
            foreach (var type in _typeClosure)
            {
                var typeDesc = UsedTypeList.Set(type);

                var layout = typeDesc.Layout.Where(kind => kind.TypeDescription.ClrTypeCode == TypeCode.Object)
                    .Select(field => field.TypeDescription.ClrType)
                    .ToArray();
                var hashSet = new HashSet<Type>(layout);
                _dictionary[type] = hashSet;
            }
        }

        public Dictionary<Type, int> ExtractInformation()
        {
            BuildDependantTypeDictionary();
            _typeClosure.Sort(this);

            _result.Clear();
            var index = 0;
            foreach (var type in _typeClosure)
            {
                _result[type] = index++;

            }
            return _result;
        }

        public int Compare(Type left, Type right)
        {
            var leftLayout = _dictionary[left];
            var rightLayout = _dictionary[right];
            if (leftLayout.Count == 0 && rightLayout.Count == 0)
                return 0;
            if (rightLayout.Contains(left))
                return -1;
            if (leftLayout.Contains(right))
                return 1;
            var countLeft = leftLayout.Count;
            var countRight = rightLayout.Count;
            if (countLeft == countRight) return 0;
            var compare = countLeft - countRight;
            return compare;
        }
    }
}
