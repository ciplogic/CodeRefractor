//CastClass Test 2 - Passes, but interface support are not yet written, so fails

using System;

namespace Obj
{
    interface Bah
    {
        int H();
    }
    class A : Bah
    {
        public int F() { return 1; }
        public virtual int G() { return 2; }
        public int H() { return 10; }
    }
    class B : A
    {
        public new int F() { return 3; }
        public override int G() { return 4; }
        public new int H() { return 11; }
    }
    class Test
    {
        static public void Main()
        {
            int result = 0;
            B b = new B();
            A a = b;

            Console.WriteLine(a.H());
            Console.WriteLine(((A)b).H());
            Console.WriteLine(((B)a).H());

            Console.WriteLine(result);
        }
    };
};
