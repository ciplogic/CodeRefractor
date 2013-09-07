using System;
using System.Linq;
using System.Reflection;

namespace CodeRefractor.RuntimeBase.Analyze
{
    static class ClassHierarchyAnalysis
    {
        public static
            MethodBase GetBestVirtualMatch(MethodBase method, Type instance)
        {
            if (method.DeclaringType == instance)
                return method;
            var methods = instance.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                .Where(m => method.Name==m.Name)
                .ToArray();
            var searchParams = method.GetParameters();
            foreach (var methodInfo in methods)
            {
                var compareParameters = methodInfo.GetParameters();
                if(searchParams.Length!=compareParameters.Length)
                    continue;
                var matching = true;
                for (var index = 0; index < compareParameters.Length; index++)
                {
                    var searchParam = searchParams[index];
                    var compareParameter = compareParameters[index];
                    if(searchParam.ParameterType!=compareParameter.ParameterType)
                    {
                        matching = false;
                        break;
                    }
                }
                if (matching)
                    return methodInfo;

            }
            return method;
        }
    }
}
