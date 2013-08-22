using System;

class NBody
{
    public static double Sum(double a, double b)
    {
        return a + Math.Sin(b);
    }
    public static void Main()
    {
        var a = 2.0;
        var b = 3.0;
        var c =Sum(a, b);
        Console.WriteLine(c);
    }
}