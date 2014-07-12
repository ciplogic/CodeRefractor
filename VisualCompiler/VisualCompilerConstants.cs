namespace VisualCompiler
{
    static class VisualCompilerConstants
    {
        public static string InitialCode = @"using System;

class Primes
{



 public static void Main()
    {
        Console.WriteLine(""Prime numbers: "".Length); 
        var len = 1000000;
        var primes = AddPrimes(len);
        Console.Write(primes);

    }

    private static int AddPrimes(int len)
    {
        var primes = 0;
        for (var i = 2; i < len; i++)
        {
            if (i%2 == 0)
                continue;
            var isPrime = true;
            for (var j = 2; j*j <= i; j++)
            {
                if (i%j == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            if (isPrime)
                primes++;
        }
        return primes;
    }
    
}";
    }
}