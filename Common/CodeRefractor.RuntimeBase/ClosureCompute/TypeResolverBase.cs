#region Uses

using System;

#endregion

namespace CodeRefractor.ClosureCompute
{
    public abstract class TypeResolverBase
    {
        public abstract Type Resolve(Type type);
    }
}