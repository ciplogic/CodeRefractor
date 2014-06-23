//Was hanging compiler on multiple trampolines ... now fixed
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
class C : B
{
    new public void F() { Console.WriteLine("C.F"); }
    public override void G() { Console.WriteLine("C.G"); }
}
class Test
{
    static void Main()
    {
        C c = new C();
        B b = c;
        A a = b;
        a.F();
        b.F();
        c.F();
        a.G();
        b.G();
        c.G(); // -  causes crash ... resolution issue ? Fixed
    }
}