using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.ClosureCompute.Steps.AddTypes
{
    public class AddParameterTypesToClosure : ClosureComputeBase
    {
        public override bool UpdateClosure(ClosureEntities closureEntities)
        {
            var methods = closureEntities.MethodImplementations.Keys;
            var result = false;
            foreach (var method in methods.Select(mk=>mk.Method))
            {
                result |= UpdateClosureForMethod(method, closureEntities);
            }
            return result;
        }

        private static bool UpdateClosureForMethod(MethodBase method, ClosureEntities closureEntities)
        {
            var result = false;
            var returnType = method.GetReturnType();
            if (returnType != typeof(void))
                result |= closureEntities.AddType(returnType);

            result |= closureEntities.AddType(method.DeclaringType);

            var parameters = method.GetParameters();
            foreach (var parameter in parameters)
            {
                result |= closureEntities.AddType(parameter.ParameterType);
            }
            return result;
        }
    }
}