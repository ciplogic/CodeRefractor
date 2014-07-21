//Tests abstract classes, abstract and overidden properties too
abstract class Shape
{
    public abstract double Area
    {
        get;
        set;
    }
}

class Square : Shape
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

class Cube : Shape
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

class TestShapes
{
    static void Main()
    {
        // Input the side:
        System.Console.Write("Enter the side: ");
        double side = 5;//double.Parse(System.Console.ReadLine());

        // Compute the areas:
        Square s = new Square(side);
        Cube c = new Cube(side);

        // Display the results:
        System.Console.Write("Area of the square =");
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
        System.Console.WriteLine(s.side);
        System.Console.Write("Side of the cube = ");
        System.Console.WriteLine(c.side);
    }
}