#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd.Interpreters.Cil;

#endregion

namespace CodeRefractor.ClosureCompute
{
    /// <summary>
    ///     Are various algorithms which find if new types
    ///     or methods are added to the CR closure
    /// </summary>
    public abstract class ClosureComputeBase
    {
        public abstract bool UpdateClosure(ClosureEntities closureEntities);

        protected static void AddMethodToClosure(ClosureEntities closureEntities, MethodBase method)
        {
            var interpreter = closureEntities.ResolveMethod(method) ?? new CilMethodInterpreter(method);
            var intepreter = interpreter as CilMethodInterpreter;
            if (intepreter != null)
            {
                intepreter.Process(closureEntities);
            }
            closureEntities.UseMethod(method, interpreter);
        }
    }
}