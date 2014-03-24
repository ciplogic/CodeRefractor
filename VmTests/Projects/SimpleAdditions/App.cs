using System;
using System.IO;
using Tao.OpenGl;
using Tao.Sdl;

namespace Game
{
    public class App
    {
      

        [STAThread]
        public static void Main()
        {
            var text = File.ReadAllText("output.cpp");
            Console.WriteLine(text);
        }
    }
}