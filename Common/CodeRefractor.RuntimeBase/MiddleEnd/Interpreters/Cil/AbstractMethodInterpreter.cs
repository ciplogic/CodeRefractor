using System.Reflection;
using CodeRefractor.FrontEnd.SimpleOperations.Methods;

namespace CodeRefractor.MiddleEnd.Interpreters.Cil
{
    public class AbstractMethodInterpreter : MethodInterpreter
    {
        public AbstractMethodInterpreter(MethodBase method) : base(method)
        {
            Kind = MethodKind.Abstract;

        }
    }
}