using System;
using System.Collections.Generic;
using System.Linq;
using CodeRefractor.RuntimeBase.Analyze;

namespace CodeRefractor.RuntimeBase
{
    public class ClosureTypeComparer : IComparer<Type>
    {
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
            var leftTypeDesc = UsedTypeList.Set(left);
            var rightTypeDesc = UsedTypeList.Set(right);

            var leftLayout = leftTypeDesc.Layout.Where(kind => kind.TypeDescription.ClrTypeCode == TypeCode.Object).ToList();
            var rightLayout = rightTypeDesc.Layout.Where(kind => kind.TypeDescription.ClrTypeCode == TypeCode.Object).ToList(); 
            if (leftLayout.Count == 0 && rightLayout.Count == 0)
                return 0;
            var leftTypeInRight = rightLayout.FirstOrDefault(kind => kind.TypeDescription.ClrType == left);
            if (leftTypeInRight != null)
                return -1;
            var rightTypeInLeft = leftLayout.FirstOrDefault(kind => kind.TypeDescription.ClrType == right);
            if (rightTypeInLeft != null)
                return 1;
            var countLeft = leftLayout.Count(kind => kind.TypeDescription.ClrTypeCode == TypeCode.Object);
            var countRight = rightLayout.Count(kind => kind.TypeDescription.ClrTypeCode == TypeCode.Object);
            if (countLeft != countRight)
            {
                var compare = countLeft-countRight;
                return compare;
            }
            return 0;
        }
    }
}