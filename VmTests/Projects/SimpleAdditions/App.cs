using System;

namespace SimpleAdditions
{
    public class ImplBase 
    {
        public void ToImplement(int value)
        {
            Console.WriteLine(value);
        }

    }
    public static class App
    {
        [STAThread]
        public static void Main()
        {
            var impl = new ImplBase();

            impl.ToImplement(2);
        }
    }
}