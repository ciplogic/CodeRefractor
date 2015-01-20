//Taken From Microsoft Documentation on Virtual Methods
using System;

class A
{
    public void F() { Console.WriteLine("A.F"); }
    public virtual void G() { Console.WriteLine("A.G"); }
}

class B : A
{
    new public void F() { Console.WriteLine("B.F"); }
    public override void G() { Console.WriteLine("B.G"); }
}

/**
 * This is a comment
 * TODO
: this is a todo thing
 */
class MyClass
{
    public void Display(int value)
    {
        Console.WriteLine(value+3);
    }
}
public class Test
{
    public static void Main()
    {
        var cl = new MyClass();
        cl.Display(3);
    }
}