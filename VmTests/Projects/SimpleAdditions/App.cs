using System;
using Tao.OpenGl;
using Tao.Sdl;

namespace Game
{
    public interface IBaseInterface
    {
        void InterfaceMethod();
    }
    public abstract class Base
    {
        public abstract void ToImplement();
    }

    public class ImplBaseB : Base
    {
        public override void ToImplement()
        {
            Console.WriteLine("ImplBaseB");
        }
    }
    public class ImplBase : Base, IBaseInterface
    {
        public override void ToImplement()
        {
            Console.WriteLine("ImplBase");
        }

        public void InterfaceMethod()
        {
            Console.WriteLine("ImplBase.IBaseInterface");
        }
    }
    public class App
    {
        [STAThread]
        public static void Main()
        {
            var impls = new Base[2];
            var impl = new ImplBase();
            impls[0] = impl;
            var implB = new ImplBaseB();
            impls[1] = implB;
            foreach (var aBase in impls)
            {
                aBase.ToImplement();
            }
            impl.InterfaceMethod();
        }
    }
}