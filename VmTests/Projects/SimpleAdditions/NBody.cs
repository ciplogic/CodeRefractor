
using System;

namespace Figures
{
    class Figure
    {
        public virtual double Area()
        {
            return 0;
        }
    }
    class Circle : Figure
    {
        public override double Area()
        {
            return 2;
        }
    }

    class Program
    {
        public static void Main(string[] args)
        {
            var c = new Circle();
            var area = c.Area();
        }
    }
}