using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;

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