using System;

class NBody
{
    public static void Main()
    {
        Body.bodyCount++;
    }
}

class Body
{
    public double x, y, z, vx, vy, vz, mass;
    public static int bodyCount;
}
           