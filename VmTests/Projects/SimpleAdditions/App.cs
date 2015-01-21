//Taken From Microsoft Documentation on Virtual Methods
using System;
struct Counter
{
    public int i;
}

struct Something
{
    public Counter c;
}
class Test
{
    static void Main()
    {
        Something s;
        s.c.i = 0;
        s.c.i++;
    }
}