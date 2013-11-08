//~~~~~~App.cs~~~~~~

using System;

class NBody
{
    public delegate void OnCall(int data);

    public static void Main()
    {
        OnCall toCall = DoCall;

        toCall.Invoke(3);
    }

    private static void DoCall(int data)
    {
        Console.WriteLine(data);
    }
}
