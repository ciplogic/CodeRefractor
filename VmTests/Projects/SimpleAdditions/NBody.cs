//~~~~~~App.cs~~~~~~

using System;


class Test
{
    static Test()
    {
        X = 3;
    }

    public static int X { get; set; }
}

public class Program
{
    public static void Main()
    {
        var test = new Test();
        Console.WriteLine(Test.X);
    }
}
