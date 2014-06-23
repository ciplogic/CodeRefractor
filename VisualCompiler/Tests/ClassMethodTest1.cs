//Class Method Test 1  - base class calls working
using System;

class A
{
    public virtual void Test()
    {
        Console.WriteLine("A.Test");
    }
}

class B : A
{
    public override void Test()
    {
        Console.WriteLine("B.Test");
        base.Test();
    }
}

//If we add C order of definitions becomes a problem

class Program
{
    static void Main()
    {


        // Compile-time type is A.
        // Runtime type is B.
        A ref2 = new B();
        ref2.Test();


    }
}