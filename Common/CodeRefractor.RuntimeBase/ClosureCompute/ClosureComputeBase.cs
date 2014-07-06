#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public abstract class ClosureComputeBase
    {
        public abstract bool UpdateClosure(ClosureEntities closureEntities);


        protected static void AddMethodToClosure(ClosureEntities closureEntities, MethodBase method)
        {
            var interpreter = closureEntities.ResolveMethod(method) ?? new CilMethodInterpreter(method);
            var intepreter = interpreter as CilMethodInterpreter;
            if (intepreter != null)
            {
                intepreter.Process();
            }
            closureEntities.UseMethod(method, interpreter);
        }
    }
}