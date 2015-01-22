using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeRefractor.Util
{
    /**
     * A utility class that returns the once value once, and it will keep
     * returning the many value on subsequent calls.
     */
    public class OnceMany<T>
    {
        private readonly T _once;
        private readonly T _many;

        private bool _firstReturned = false;

        public OnceMany(T once, T many)
        {
            this._once = once;
            this._many = many;
        }

        /**
         * Returns the once value the once time the method is called,
         * and keeps returning the many value afterwards.
         */
        public T next()
        {
            if (!_firstReturned)
            {
                _firstReturned = true;
                return _once;
            }

            return _many;
        }
    }
}
