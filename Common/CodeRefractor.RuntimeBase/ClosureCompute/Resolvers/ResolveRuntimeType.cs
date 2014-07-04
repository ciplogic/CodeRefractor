using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.RuntimeBase.Shared;

namespace CodeRefractor.ClosureCompute.Resolvers
{
    public class ResolveRuntimeType : TypeResolverBase
    {
        private readonly Dictionary<Type, Type> _solvedTypes;

        public ResolveRuntimeType(Assembly assembly)
        {
            _solvedTypes = assembly.GetTypes()
                .Where(t => ((Type) t).GetCustomAttribute<MapTypeAttribute>() != null)
                .ToDictionary(
                    tp => tp.GetCustomAttribute<MapTypeAttribute>().MappedType
                );
        }

        public override Type Resolve(Type method)
        {
            Type result;
            if (!_solvedTypes.TryGetValue(method, out result))
                return null;
            return result;
        }
    }
}