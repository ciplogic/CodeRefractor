#region Uses

using System;
using System.Reflection;
using CodeRefractor.ClosureCompute.Resolvers;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public class ClosureEntitiesUtils
    {
        readonly ClosureEntities _getClosureEntities;

        public ClosureEntitiesUtils(ClosureEntities getClosureEntities)
        {
            _getClosureEntities = getClosureEntities;
        }

        public ClosureEntities BuildClosureEntities(MethodInfo definition, Assembly runtimeAssembly)
        {
            var closureEntities = _getClosureEntities;

            closureEntities.EntryPoint = definition;

            var resolveRuntimeMethod = new ResolveRuntimeMethod(runtimeAssembly, closureEntities);
            closureEntities.AddMethodResolver(resolveRuntimeMethod);

            closureEntities.AddMethodResolver(new ResolvePlatformInvokeMethod());

            var extensionsResolverMethod = new ResolveRuntimeMethodUsingExtensions(runtimeAssembly, closureEntities);
            closureEntities.AddMethodResolver(extensionsResolverMethod);

            closureEntities.EntitiesBuilder.AddTypeResolver(new ResolveRuntimeType(runtimeAssembly));

            closureEntities.ComputeFullClosure();
            closureEntities.OptimizeClosure(closureEntities);

            return closureEntities;
        }
    }
}