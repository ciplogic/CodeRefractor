using System;
using System.Runtime.InteropServices;
using System.Text;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using Tao.OpenGl;
using Tao.Sdl;

namespace SimpleAdditions
{
    class Program
    {
        public static void Main()
        {
            var text = "one";
            var text2 = "two";
            var text3 = "three";
            var strings = new string[]{text, text2};
            Array.Resize(ref strings, 3);
            strings[2] = text3;
        }
    }
}