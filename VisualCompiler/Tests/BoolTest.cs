public static class Program
{


    public static void Main()
    {
        //This works
        bool result = (1 == 2);

        if (result)
            System.Console.WriteLine("True");
        else
            System.Console.WriteLine("False");

        //This doesn't, now works added Console.WriteLine(bool)
        System.Console.WriteLine(1 == 2);
    }
}