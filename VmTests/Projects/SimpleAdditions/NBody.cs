
using System;
using System.Text;

namespace Figures
{
    class Program
    {
        public static int OsNewsBench32(int maxValue)
        {
            var result = 1;
            for(var i = 1; i< maxValue; i+=4)
            {
                result = -i + (i + 1)*(i + 2)/(i + 3);
            }
            return result;
        }
        public static void Main()
        {
            var compute = OsNewsBench32(1000000000);
            Console.WriteLine(compute);
        }
    }
}