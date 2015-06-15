#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.ClosureCompute.Resolvers
{
    public class ResolveRuntimeMethodUsingExtensions : MethodResolverBase
    {
        private readonly ClosureEntities _closureEntities;
        private readonly Dictionary<Type, List<MethodInfo>> _solvedTypes;

        public ResolveRuntimeMethodUsingExtensions(Assembly assembly, ClosureEntities closureEntities)
        {
            _closureEntities = closureEntities;
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
                var typesData = implementation.GetCustomAttribute<ExtensionsImplementation>();
                foreach (var methodInfo in allMethods)
                {
                    var attributeMethod = methodInfo.GetCustomAttributeT<MapMethod>();
                    if (attributeMethod == null)
                        continue;
                    var declaringType = typesData.DeclaringType;
                    AddMethod(declaringType, methodInfo);
                }
            }
        }

        private void AddMethod(Type declaringType, MethodInfo methodInfo)
        {
            List<MethodInfo> list;
            if (!_solvedTypes.TryGetValue(declaringType, out list))
            {
                list = new List<MethodInfo>();
                _solvedTypes[declaringType] = list;
            }
            list.Add(methodInfo);
        }

        public override MethodInterpreter Resolve(MethodBase method)
        {
            List<MethodInfo> list;
            if (!_solvedTypes.TryGetValue(method.DeclaringType, out list))
                return null;

            var resultMethod = ResolveRuntimeMethod.CalculateResultMethod(method, list, _closureEntities);
            if (resultMethod == null)
                return null;
            return ResolveRuntimeMethod.ResolveMethodWithResult(resultMethod, method.DeclaringType);
        }
    }
}