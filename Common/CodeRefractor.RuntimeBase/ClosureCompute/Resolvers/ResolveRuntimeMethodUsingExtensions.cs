using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.ClosureCompute.Resolvers
{
    public class ResolveRuntimeMethodUsingExtensions : MethodResolverBase
    {
        private readonly Dictionary<Type, List<MethodInfo>> _solvedTypes;
        public ResolveRuntimeMethodUsingExtensions(Assembly assembly)
        {
            var extensionImplementations = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<ExtensionsImplementation>() != null)
                .ToList();
            _solvedTypes = new Dictionary<Type, List<MethodInfo>>();
            PopulateSolvedTypes(extensionImplementations);
        }

        private void PopulateSolvedTypes(List<Type> extensionImplementations)
        {
            foreach (var implementation in extensionImplementations)
            {
                var allMethods = implementation.GetMethods();
                foreach (var methodInfo in allMethods)
                {
                    var attributeMethod = methodInfo.GetCustomAttributeT<MapMethod>();
                    if (attributeMethod == null)
                        continue;
                    var declaringType = attributeMethod.DeclaringType??methodInfo.GetParameters()[0].ParameterType;
                    AddMethod(declaringType, methodInfo);
                }
            }
        }

        void AddMethod(Type declaringType, MethodInfo methodInfo)
        {
            List<MethodInfo> list;
            if (!_solvedTypes.TryGetValue(declaringType, out list))
            {
                list =new List<MethodInfo>();
                _solvedTypes[declaringType] = list;
            }
            list.Add(methodInfo);
        }

        public override MethodInterpreter Resolve(MethodBase method)
        {
            List<MethodInfo> list;
            if (!_solvedTypes.TryGetValue(method.DeclaringType, out list))
                return null;

            var resultMethod = CalculateResultMethod(method, list);
            if (resultMethod == null)
                return null;
            var result = new MethodInterpreter(resultMethod);

            return result;
        }


        private static MethodInfo CalculateResultMethod(MethodBase method, List<MethodInfo> allMethods)
        {
            MethodInfo resultMethod = null;
            var srcParams = method.GetParameters().Select(par=>par.ParameterType).ToList();
            if (!method.IsStatic)
            {
                srcParams.Insert(0, method.DeclaringType);
            }
            foreach (var methodInfo in allMethods)
            {
                var attributeMethod = methodInfo.GetCustomAttributeT<MapMethod>();
                attributeMethod.Name = string.IsNullOrEmpty(attributeMethod.Name)
                    ? methodInfo.Name
                    : attributeMethod.Name;
                if(attributeMethod.Name!=method.Name)
                    continue;
                var targetParams = methodInfo.GetParameters().ToList();
                if (srcParams.Count != targetParams.Count)
                    continue;
                resultMethod = methodInfo;
                var found = true;
                for (var index = 0; index < srcParams.Count; index++)
                {
                    var param = srcParams[index];
                    var targetParam = targetParams[index];
                    if (param == targetParam.ParameterType) continue;
                    found = false;
                    break;
                }
                if (!found)
                    resultMethod = null;
            }
            return resultMethod;
        }

        
    }
}