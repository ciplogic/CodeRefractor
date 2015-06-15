#region Uses

using System;
using System.Collections.Generic;
using System.Text;

#endregion

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
            var result = new StringBuilder("");
            var comma = new OnceMany<string>("", separator);

            foreach (var item in enumerable)
            {
                result.Append(comma.next())
                    .Append(item);
            }

            return result.ToString();
        }

        /**
         * Calls the given action on all the elements but the first element.
         * Returns the full enumerable.
         */

        public static IEnumerable<T> ButFirst<T>(this IEnumerable<T> enumerable, Action<T> action)
        {
            var first = true;

            foreach (var item in enumerable)
            {
                if (first)
                {
                    first = false;
                    continue;
                }

                action.Invoke(item);
            }

            return enumerable;
        }

        /**
         * Calls the given action once, and returns the enumerable.
         */

        public static IEnumerable<T> Once<T>(this IEnumerable<T> enumerable, Action<IEnumerable<T>> action)
        {
            action.Invoke(enumerable);

            return enumerable;
        }
    }
}