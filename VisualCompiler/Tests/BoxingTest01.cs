using System;

class BoxingTest01
{

    private static void Log(object data)
    {
        //This works
        int a = (int)data;
        Console.WriteLine("Integer:" + a.ToString());

        //This doesnt (Need proper isinst instruction)
        if (data is int)
        {
            a = (int)data;
            Console.WriteLine("Integer:" + a.ToString());
        }

    }

    public static void Main()
    {
        int n = 500000;
        Log(n);
    }
}