/* The Computer Language Benchmarks Game
   http://benchmarksgame.alioth.debian.org/

   contributed by Isaac Gouy, optimization and use of more C# idioms by Robert F. Tobler
*/

using System;

namespace SimpleAdditions
{
    class FixedList<T>
    {
        public T []_items;
        public FixedList(int count)
        {
            _items=new T[count];
        } 
    }
    class NBody
    {
        
        public static void Main()
        {
            var arr = new FixedList<int>(3);
            arr._items[2] = 5;
            Console.WriteLine(arr._items[2]);
        }
    }
}