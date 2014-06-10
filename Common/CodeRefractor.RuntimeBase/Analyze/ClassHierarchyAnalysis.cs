#region Usings

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    internal static class ClassHierarchyAnalysis
    {
        public static
            MethodBase GetBestVirtualMatch(MethodBase method, Type instance)
        {
            if (method.DeclaringType == instance)
                return method;
            var methodsToSearch = new List<MethodBase>();
            methodsToSearch.AddRange(instance.GetMethods(BindingFlags.Instance | BindingFlags.Public));
            methodsToSearch.AddRange(instance.GetMethods(BindingFlags.Static | BindingFlags.Public));
            methodsToSearch.AddRange(instance.GetConstructors());
            var methods = methodsToSearch
                .Where(m => method.Name == m.Name)
                .ToArray();
            var searchParams = method.GetParameters();
            foreach (var methodInfo in methods)
            {
                var compareParameters = methodInfo.GetParameters();
                if (searchParams.Length != compareParameters.Length)
                    continue;
                var matching = true;
                for (var index = 0; index < compareParameters.Length; index++)
                {
                    var searchParam = searchParams[index];
                    var compareParameter = compareParameters[index];
                    if (searchParam.ParameterType != compareParameter.ParameterType)
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