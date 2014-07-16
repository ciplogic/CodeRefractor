using System.Reflection;
using CodeRefractor.ClosureCompute.Resolvers;

namespace CodeRefractor.ClosureCompute
{
    public static class ClosureEntitiesUtils
    {
        public static ClosureEntities BuildClosureEntities(MethodInfo definition, Assembly runtimeAssembly)
        {
            var closureEntities = new ClosureEntities { EntryPoint = definition };
            var resolveRuntimeMethod = new ResolveRuntimeMethod(runtimeAssembly);
            closureEntities.AddMethodResolver(resolveRuntimeMethod);

            closureEntities.AddMethodResolver(new ResolvePlatformInvokeMethod());

            var extensionsResolverMethod = new ResolveRuntimeMethodUsingExtensions(runtimeAssembly);
            closureEntities.AddMethodResolver(extensionsResolverMethod);

            closureEntities.TypeResolverList.Add(new ResolveRuntimeType(runtimeAssembly));

            closureEntities.ComputeFullClosure();
            closureEntities.OptimizeClosure();
            return closureEntities;
        }

    }
}