#region Uses

using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.NonCil;

#endregion

namespace CodeRefractor.ClosureCompute.Resolvers
{
    public class ResolvePlatformInvokeMethod : MethodResolverBase
    {
        public override MethodInterpreter Resolve(MethodBase method)
        {
            if (!PlatformInvokeMethod.IsPlatformInvoke(method))
                return null;
            return new PlatformInvokeMethod(method);
        }
    }
}