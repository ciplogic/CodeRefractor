using System;
using System.Collections.Generic;
using System.Text;

namespace CodeRefractor.Util
{
    /**
     * A set of enumeration/collections utility external methods.
     */
    public static class EnumerationUtils
    {
        /**
         * Iterates over the enumerable, returning the enumerable itself.
         */
        public static IEnumerable<T> Each<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            foreach (var item in enumerable)
            {
                action.Invoke(item);
            }

            return enumerable;
        }

        /**
         * Joins all the objects from the enumerable with the given separator.
         */
        public static string Join<T>(this IEnumerable<T> enumerable, string separator = ",")
        {
            StringBuilder result = new StringBuilder("");
            OnceMany<string> comma = new OnceMany<string>("", separator);

            foreach (var item in enumerable)
            {
                result.Append(comma.next())
                    .Append(item);
            }

            return result.ToString();
        }
    }
}
