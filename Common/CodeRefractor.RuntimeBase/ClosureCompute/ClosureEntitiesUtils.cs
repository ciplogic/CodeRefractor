using System;
using System.Linq;
using System.Reflection;
using CodeRefractor.ClosureCompute.Resolvers;
using CodeRefractor.RuntimeBase;
using CodeRefractor.Util;

namespace CodeRefractor.ClosureCompute
{
    public class ClosureEntitiesUtils
    {
        private Func<ClosureEntities> _getClosureEntities;

        public ClosureEntitiesUtils(Func<ClosureEntities> getClosureEntities)
        {
            this._getClosureEntities = getClosureEntities;
        }

        public ClosureEntities BuildClosureEntities(MethodInfo definition, Assembly runtimeAssembly)
        {
            var closureEntities = _getClosureEntities();

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