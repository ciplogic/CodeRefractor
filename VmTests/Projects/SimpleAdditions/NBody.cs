using System;
class Point
{
    private double _x;
    public double X
    {
        get { return _x; }
        set { _x = value; }
    }

    private double _y;
    public double Y
    {
        get { return _y; }
        set { _y = value; }
    }
}
class NBody
{

    public static void Main()
    {
        var p = new Point {X = 2, Y = 3};
    }
}