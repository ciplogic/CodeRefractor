using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.RuntimeBase
{
    public class ClosureTypeComparer : IComparer<Type>
    {
        private readonly List<Type> _typesToSort;
        Dictionary<Type, HashSet<Type>> _dictionary = new Dictionary<Type, HashSet<Type>>(); 
        public ClosureTypeComparer(List<Type> typesToSort)
        {
            _typesToSort = typesToSort;
            foreach (var type in typesToSort)
            {
                var typeDesc = UsedTypeList.Set(type);

                var layout = typeDesc.Layout.Where(kind => kind.TypeDescription.ClrTypeCode == TypeCode.Object)
                    .Select(field =>field.TypeDescription.ClrType)
                    .ToArray();
                var hashSet = new HashSet<Type>(layout);
                _dictionary[type] = hashSet;
            }
        }

        public void Sort()
        {
            _typesToSort.Sort(this);
        }

        public int Compare(Type left, Type right)
        {
            if (left.IsValueType && !right.IsValueType)
            {
                return -1;
            }
            if (right.IsValueType && !left.IsValueType)
            {
                return 1;
            }
            var leftLayout = _dictionary[left];
            var rightLayout = _dictionary[right]; 
            if (leftLayout.Count== 0 && rightLayout.Count== 0)
                return 0;
            if (rightLayout.Contains(left))
                return -1;
            if (leftLayout.Contains(right))
                return 1;
            var countLeft = leftLayout.Count;
            var countRight = rightLayout.Count;
            if (countLeft == countRight) return 0;
            var compare = countLeft-countRight;
            return compare;
        }
    }
}