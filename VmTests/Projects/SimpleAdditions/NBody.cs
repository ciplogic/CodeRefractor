//~~~~~~App.cs~~~~~~

using System;
using System.Runtime.InteropServices;
using Tao.Sdl;

class NBody
{
    public delegate void OnCall(int data);

    public static void Main()
    {
        OnCall toCall = DoCall;

    }

    private static void DoCall(int data)
    {
        Console.WriteLine(data);
    }
}
