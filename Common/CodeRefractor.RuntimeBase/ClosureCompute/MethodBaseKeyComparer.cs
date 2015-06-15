#region Uses

using System.Collections.Generic;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public class MethodBaseKeyComparer : IEqualityComparer<MethodBaseKey>
    {
        public bool Equals(MethodBaseKey x, MethodBaseKey y)
        {
            return CompareEquals(x, y);
        }

        public int GetHashCode(MethodBaseKey obj)
        {
            return obj.GetHashCode();
        }

        public static bool CompareEquals(MethodBaseKey x, MethodBaseKey y)
        {
            var result = x.Method.MethodMatches(y.Method);
            return result;
        }
    }
}