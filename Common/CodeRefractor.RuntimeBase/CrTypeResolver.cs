using System;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.RuntimeBase
{
    public abstract class CrTypeResolver
    {
        public abstract bool Resolve(MethodInterpreter methodInterpreter);

        public abstract Type ResolveType(Type type);
    }
}