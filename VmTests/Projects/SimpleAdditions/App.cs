//http://www.dotnetperls.com/interface

using System;
using IDisp;

namespace IDisp
{
    internal class PersistentData : IDisposable
    {
        private int enabled = 1;

        public PersistentData()
        {
            Console.WriteLine("Enabled");
        }

        public void Dispose()
        {
            enabled = 0;
            Console.WriteLine("Not enabled");

        }
    }
}

class Program
{
    static void Main()
    {
        try
        {
            Console.WriteLine("content");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Inside catch");
        }
        finally
        {

            Console.WriteLine("finally");
        }
        /*
        IValue value1 = new Image();
        IValue value2 = new Article();

        value1.Count++; // Access int property on interface
        value2.Count++; // Increment

        value1.Name = "Mona Lisa"; // Use setter on interface
        value2.Name = "Resignation"; // Set

        Console.WriteLine(value1.Name); // Use getter on interface
        Console.WriteLine(value2.Name); // Get
         */
    }
}