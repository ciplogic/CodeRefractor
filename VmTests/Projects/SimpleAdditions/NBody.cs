using System;

class Test
{
    public void F1()
    {
        var sum = F2(2, 4);
        Console.WriteLine(sum);
    }

    public static int F2(int a, int b)
    {
        return a + b;
    }
}

class NBody
{
    public static void Main()
    {
        var test = new Test();
        test.F1();
    }
}