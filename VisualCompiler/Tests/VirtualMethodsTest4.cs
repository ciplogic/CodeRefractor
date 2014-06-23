//Virtual Test 4 - Causes new issues, function changes semantics when method not overriden
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
    }
}

class C : B
{
    public override void Test()
    {
        Console.WriteLine("C.Test");
    }
}

class D : C
{
    // no dispatch if Test is declared as "new" // do we have to keep a typeid to struct's base class and run that when no match is found ? //Fixed
}


//If we add C order of definitions becomes a problem

class Program
{
    static void Main()
    {
        // Compile-time type is A.
        // Runtime type is A as well.
        A ref1 = new A();
        ref1.Test();

        // Compile-time type is A.
        // Runtime type is B.
        A ref2 = new B();
        ref2.Test();

        // Compile-time type is A.
        // Runtime type is B.
        A ref3 = new C();
        ref3.Test();


        // Compile-time type is A.
        // Runtime type is B.
        A ref4 = new D();
        ref4.Test();

    }
}