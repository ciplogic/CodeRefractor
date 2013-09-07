
using System;
using System.Text;

namespace Figures
{
    class Program
    {
        public static void Main()
        {
            var charData = new[] {'H','e','l','l','o',' ','w','o','r','l','d'};
            var s = new string(charData);
            Console.WriteLine(s);
        }
    }
}