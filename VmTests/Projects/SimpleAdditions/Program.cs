using System;
using System.Runtime.InteropServices;
using System.Text;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
namespace SimpleAdditions
{
    public class AuxList<T>
    {
        public T[] Data;

        public int Count { get; private set; }

        public int Capacity { get { return Data.Length; } }
        public AuxList(int capacity = 0)
        {
            Data = new T[capacity];
        }

        public void Add(T item)
        {
            if (Count <= Capacity)
            {
                if (Capacity == 0)
                {
                    Array.Resize(ref Data, 10);
                }
            }
            Data[Count] = item;
            Count++;
        }

        public T Get(int index)
        {
            return Data[index];
        }
    }
    class Program
    {
        public static void Main()
        {
            var list = new AuxList<int>();
            list.Add(2);
            var item = list.Get(0);
            Console.WriteLine(item);
        }
    }
}