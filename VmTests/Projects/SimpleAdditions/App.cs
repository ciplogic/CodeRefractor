using System;
using Tao.OpenGl;
using Tao.Sdl;

namespace Game
{
    class MyList<T>
    {
        T[] items = new T[10];
        public int Capacity { get; set; }
        public int Count { get; private set; }
        public MyList()
        {
            
        } 
        public void Add(T item)
        {
            items[Count] = item;
            Count++;
        }
    }
    public class App
    {
        [STAThread]
        public static void Main()
        {
            var list = new MyList<int>();
            list.Add(2);
        }
    }
}