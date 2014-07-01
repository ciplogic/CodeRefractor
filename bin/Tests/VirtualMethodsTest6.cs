//Virtual Test 6 - lets use various parameter and return types, support method overloads
using System;

class A
{
    public virtual void Test()
    {
        Console.WriteLine("A.Test");
    }


    public virtual void Test(string message)
    {
        Console.WriteLine(message);
    }

    public virtual B NotAnA(int count, string message)
    {
        Console.WriteLine(count);
        Console.WriteLine(message);
        Console.WriteLine("A.Test");
        return new B();
    }

}

class B : A
{
    public override void Test()
    {
        Console.WriteLine("B.Test");
    }

    public override void Test(string message)
    {
        Console.WriteLine(message);
    }

    public override B NotAnA(int count, string message)
    {
        Console.WriteLine(count);
        Console.WriteLine(message);
        Console.WriteLine("A.Test");
        return this;
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
        var notA = ref1.NotAnA(0, "test1");
        notA.Test("gut");

        // Compile-time type is A.
        // Runtime type is B.
        A ref2 = new B();
        ref2.Test();

        var notA2 = ref2.NotAnA(0, "test2");
        notA2.Test();

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
