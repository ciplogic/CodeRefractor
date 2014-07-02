#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public abstract class MethodResolverBase
    {
        public abstract MethodInterpreter Resolve(MethodBase method);
    }
}