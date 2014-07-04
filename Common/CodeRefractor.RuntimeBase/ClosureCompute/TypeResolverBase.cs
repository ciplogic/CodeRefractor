using System;

namespace CodeRefractor.ClosureCompute
{
    public abstract class TypeResolverBase
    {
        public abstract Type Resolve(Type method);
    }
}