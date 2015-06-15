using System.Collections.Generic;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime.Generic
{
    [MapType(typeof(List<>))]
    class CrList<T>
    {
        T[] _items;

        public int Count { get; set; }
        public int Capacity
        {
            get { return _items.Length; }
            private set
            {
                var capacity = value;
                if (_items == null)
                {
                    _items = new T[capacity];
                }
                else
                {
                    MyArray.Resize(ref _items, capacity);
                }
            }
        }

        public CrList()
        {
            Capacity = 8;
        }

        public void Add(T value)
        {
            var count = Count;
            if (count == Capacity)
            {
                Capacity *= 2;
            }
            _items[count] = value;
            Count = count + 1;
        }
    }

    public class MyArray
    {
        public static void Resize<T>(ref T[] array, int newSize)
        {
            if (array == null)
            {
                array = new T[newSize];
                return;
            }
            if (array.Length == newSize) return;
            var array3 = new T[newSize];
            Copy(array3, array, array.Length);
            array = array3;
        }

        public static void Copy<T>(T[] dest, T[] src, int size)
        {
            for (var index = 0; index < size; index++)
            {
                var item = src[index];
                dest[index] = item;
            }
        }
    }
}
