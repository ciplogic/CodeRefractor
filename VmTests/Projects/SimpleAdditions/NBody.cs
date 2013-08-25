using System;

namespace SimpleAdditions
{
    class NBody
    {
        public static double Sum(double a, double b)
        {
            return a + Math.Sin(b);
        }
        public static void Main()
        {
            var a = 2;
            var b = 3;
            var c =Sum(a, b);
            Console.WriteLine(c);
        }
    }
}