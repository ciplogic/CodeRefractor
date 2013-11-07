/* The Computer Language Benchmarks Game
   http://benchmarksgame.alioth.debian.org/

   contributed by Isaac Gouy, optimization and use of more C# idioms by Robert F. Tobler
*/

using System;

namespace SimpleAdditions
{
    class NBody
    {
        unsafe static void FillWithColor(uint* data, int w, int h, uint color)
        {
            var pixelCount = w * h;
            for (var i = 0; i < pixelCount; i++)
            {
                *data = color;
                data++;
            }
        }
        public static unsafe void Main()
        {
            var surface = new uint[800 * 600];
            fixed (uint* srf = surface)
            {
                FillWithColor(srf, 800, 600, 255);
            }
        }
    }
}