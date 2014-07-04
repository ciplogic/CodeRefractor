#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.SimpleOperations.Methods;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.ClosureCompute.Resolvers
{
    public class ResolveRuntimeMethod : MethodResolverBase
    {
        private readonly Dictionary<Type, Type> _solvedTypes;

        public ResolveRuntimeMethod(Assembly assembly)
        {
            _solvedTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<MapTypeAttribute>() != null)
                .ToDictionary(
                    tp => tp.GetCustomAttribute<MapTypeAttribute>().MappedType
                );
        }

        public override MethodInterpreter Resolve(MethodBase method)
        {
            var declaringType = method.DeclaringType;
            Type resolvingType;
            if (!_solvedTypes.TryGetValue(declaringType, out resolvingType))
            {
                return null;
            }
            var allMethods = resolvingType.GetMethods().Where(m => m.Name == method.Name).ToArray();
            var resultMethod = CalculateResultMethod(method, allMethods);

            if (resultMethod == null) return null;
            return ResolveMethodWithResult(resultMethod);
        }

        private static MethodInfo CalculateResultMethod(MethodBase method, MethodInfo[] allMethods)
        {
            MethodInfo resultMethod = null;
            var srcParams = method.GetParameters();
            foreach (var methodInfo in allMethods)
            {
                var targetParams = methodInfo.GetParameters();
                var found = DoParametersMatch(srcParams, targetParams);
                if (!found) continue;
                resultMethod = methodInfo;
                break;
            }
            return resultMethod;
        }

        private static bool DoParametersMatch(ParameterInfo[] srcParams, ParameterInfo[] targetParams)
        {
            if (srcParams.Length != targetParams.Length)
                return false;
            for (var index = 0; index < srcParams.Length; index++)
            {
                var param = srcParams[index];
                var targetParam = targetParams[index];
                if (param.ParameterType == targetParam.ParameterType) continue;
                return false;
            }
            return true;
        }

        private static MethodInterpreter ResolveMethodWithResult(MethodBase resultMethod)
        {
            var result = new MethodInterpreter(resultMethod);

            var cppAttribute = resultMethod.GetCustomAttribute<CppMethodBodyAttribute>();
            if (cppAttribute != null)
            {
                result.Kind = MethodKind.RuntimeCppMethod;
                var cppRepresentation = result.CppRepresentation;
                cppRepresentation.Kind = CppKinds.RuntimeLibrary;
                cppRepresentation.Header = cppAttribute.Header;
                cppRepresentation.Source = cppAttribute.Code;
                var pureAttribute = resultMethod.GetCustomAttribute<PureMethodAttribute>();
                if (pureAttribute != null)
                    result.AnalyzeProperties.IsPure = true;
                return result;
            }
            result.Kind = MethodKind.Default;
            
                
            return result;
        }
    }
}