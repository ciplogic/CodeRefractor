#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.Runtime;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public abstract class ClosureComputeBase
    {
        public abstract bool UpdateClosure(ClosureEntities closureEntities);


        protected static void AddMethodToClosure(ClosureEntities closureEntities, MethodBase method)
        {
            var interpreter = closureEntities.ResolveMethod(method) ?? new MethodInterpreter(method);

            interpreter.Process(new CrRuntimeLibrary());
            closureEntities.UseMethod(method, interpreter);
        }
    }
}