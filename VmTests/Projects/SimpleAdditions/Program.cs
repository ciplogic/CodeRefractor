using System;
using System.Diagnostics;

namespace SimpleAdditions
{
    class Program
    {
        static void Main()
        {
            var start = new Stopwatch();
            start.Start();
            var len = 1000000;
            var primes = 0;
            for (var i = 2; i < len; i++)
            {
                if (i % 2 == 0)
                    continue;
                var isPrime = true;
                for (var j = 2; j * j <= i; j++)
                {
                    if (i % j == 0)
                    {
                        isPrime = false;
                        break;
                    }
                }
                if (isPrime)
                    primes++;
            }
            Console.Write(primes);

            start.Stop();
            Console.WriteLine(start.ElapsedMilliseconds);
            Console.ReadKey();
        }
    }
}
