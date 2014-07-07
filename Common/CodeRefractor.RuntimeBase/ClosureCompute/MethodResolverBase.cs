#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public abstract class MethodResolverBase
    {
        public abstract MethodInterpreter Resolve(MethodBase method);
    }
}