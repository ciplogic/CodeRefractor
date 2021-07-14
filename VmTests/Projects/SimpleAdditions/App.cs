//Tests abstract classes, abstract and overridden properties too

namespace SimpleAdditions
{
    internal abstract class Shape
    {
        public abstract double Area
        {
            get;
            set;
        }
    }

    internal class Square : Shape
    {
        public double side;

        public Square(double s)  //constructor
        {
            side = s;
        }

        public override double Area
        {
            get
            {
                return side * side;
            }
            set
            {
                side = System.Math.Sqrt(value);
            }
        }
    }

    internal class Cube : Shape
    {
        public double side;

        public Cube(double s)
        {
            side = s;
        }

        public override double Area
        {
            get
            {
                return 6 * side * side;
            }
            set
            {
                side = System.Math.Sqrt(value / 6);
            }
        }
    }

    internal class TestShapes
    {
        private static void Main()
        {
            // Input the side:
            System.Console.Write("Enter the side: ");
            double side = 5;//double.Parse(System.Console.ReadLine());

            // Compute the areas:
            Square s = new Square(side);
            Cube c = new Cube(side);

            // Display the results:
            System.Console.Write("Square area =");
            System.Console.WriteLine(s.Area);
            System.Console.Write("Area of the cube =");
            System.Console.WriteLine(c.Area);
            // System.Console.WriteLine();

            // Input the area:
            System.Console.Write("Enter the area: ");
            double area = 50; //double.Parse(System.Console.ReadLine());

            // Compute the sides:
            s.Area = area;
            c.Area = area;

            // Display the results:
            System.Console.Write("Side of the square = ");
            System.Console.WriteLine((float)s.side);
            System.Console.Write("Side of the cube = ");
            System.Console.WriteLine((float)c.side);
        }
    }
}