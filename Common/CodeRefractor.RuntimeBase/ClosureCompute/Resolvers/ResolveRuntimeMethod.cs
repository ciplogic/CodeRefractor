#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.CecilUtils;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.ClosureCompute.Resolvers
{
    public class ResolveRuntimeMethod : MethodResolverBase
    {
        private readonly ClosureEntities _closureEntities;
        private readonly Dictionary<Type, Type> _solvedTypes;

        public ResolveRuntimeMethod(Assembly assembly, ClosureEntities closureEntities)
        {
            _closureEntities = closureEntities;
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
            if (method.IsConstructor)
            {
                return HandleConstructor(method, resolvingType);
            }
            var allMethods = resolvingType.GetMethods(CecilCaches.AllFlags)
                .Where(m => m.Name == method.Name)
                .ToArray();
            var resultMethod = CalculateResultMethod(method, allMethods);

            if (resultMethod == null)
            {
                return null;
            }
            return ResolveMethodWithResult(resultMethod, method.DeclaringType);
        }

        private MethodInterpreter HandleConstructor(MethodBase method, Type resolvingType)
        {
            var allConstuctors = resolvingType.GetConstructors(CecilCaches.AllFlags).ToArray();
            var methodParameters = method.GetParameters();
            foreach (var constuctor in allConstuctors)
            {
                var ctorParameters = constuctor.GetParameters();
                if (DoParametersMatch(methodParameters, ctorParameters))
                    return ResolveMethodWithResult(constuctor, resolvingType);
            }
            return null;
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

        private MethodInterpreter ResolveMethodWithResult(MethodBase resultMethod, Type overrideType)
        {
            if (!CppMethodInterpreter.IsCppMethod(resultMethod))
            {
                var result = new CilMethodInterpreter(resultMethod)
                {
                    OverrideDeclaringType = overrideType
                };
                return result;
            }
            var cppResult = new CppMethodInterpreter(resultMethod)
            {
                OverrideDeclaringType = overrideType
            };

            cppResult.SetupInternalFields(resultMethod);
            return cppResult;

        }
    }
}