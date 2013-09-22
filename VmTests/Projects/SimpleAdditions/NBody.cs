//~~~~~~App.cs~~~~~~

using System;
using System.Runtime.InteropServices;
using Tao.Sdl;
// fastcopy.cs
// compile with: /unsafe
using System;

class Test
{
    // The unsafe keyword allows pointers to be used within
    // the following method:
    static unsafe void Copy(byte[] src,
        byte[] dst, int count)
    {
        

        // The following fixed statement pins the location of
        // the src and dst objects in memory so that they will
        // not be moved by garbage collection.          
        fixed (byte* pSrc = src, pDst = dst)
        {
            //byte* ps = pSrc;
            //byte* pd = pDst;


        }
    }


    static void Main(string[] args)
    {
        byte[] a = new byte[100];
        byte[] b = new byte[100];
        for (int i = 0; i < 100; ++i)
            a[i] = (byte)i;
        Copy(a, b, 100);
        Console.WriteLine("The first 10 elements are:");
        for (int i = 0; i < 10; ++i)
        {
            var value = b[i];
            Console.Write(value);
            Console.Write(" ");
        }
        Console.WriteLine("\n");
    }
}