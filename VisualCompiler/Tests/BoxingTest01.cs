using System;

class BoxingTest01
{

    private static void Log(object data)
    {
      
        //This doesnt (Need proper isinst instruction)
        if (data is int)
        {
            var a = (int)data;
            Console.WriteLine("Integer:" + a.ToString());
        }

        else
        if (data is string)
        {
            var a = (string)data;
            Console.WriteLine("String:" + a.ToString());
        }
        else
        {
            //Needs basic reflection to work
           // Console.WriteLine("unknown:" + data.ToString());
        }

    }

    public static void Main()
    {
        int n = 500000;
        Log(n);
        Log("Hey");
        Log(1.023f);
    }
}