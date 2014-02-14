using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeRefractor.RuntimeBase.Analyze.TypeTableIndices
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

        public static bool MatchCondition(Func<Type, Type, bool> matcher, Type left, Type right, out int order)
        {
            if (matcher(left, right))
            {
                order = -1;
                return true;
            }
            if (matcher(right, left))
            {
                order = 1;
                return true;
            }
            order = 0;
            return false;
        }

        class Matcher
        {
            private readonly Type _left;
            private readonly Type _right;

            public Matcher(Type left, Type right)
            {
                _left = left;
                _right = right;
            }

            public bool Matches(Func<Type, Type, bool> matcher, out int order)
            {
                return (MatchCondition(matcher, _left, _right, out order));
            }
        }
        public int Compare(Type left, Type right)
        {
            int order;
            var matcher = new Matcher(left, right);
            if (matcher.Matches((l, r) => l == typeof (object), out order))
            {
                return order;
            }
            if (matcher.Matches((l, r) => r.BaseType == l, out order))
            {
                return order;
            }
            if (matcher.Matches((l, r) => l.IsValueType && !r.IsValueType, out order))
            {
                return order;
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