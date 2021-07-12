#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.Runtime.Annotations;
using CodeRefractor.Shared;

#endregion

namespace CodeRefractor.ClosureCompute.Resolvers
{
    public class ResolveRuntimeType : TypeResolverBase
    {
        readonly Dictionary<Type, Type> _solvedTypes;

        public ResolveRuntimeType(Assembly assembly)
        {
            _solvedTypes = assembly.GetTypes()
                .Where(t => t.GetCustomAttribute<MapTypeAttribute>() != null)
                .ToDictionary(
                    tp => tp.GetCustomAttribute<MapTypeAttribute>().MappedType
                );
        }

        public override Type Resolve(Type type)
        {
            Type result;
            if (type.IsGenericType)
            {
                var genericType = type.GetGenericTypeDefinition();
                var genericArguments = type.GetGenericArguments();
                if (!_solvedTypes.TryGetValue(genericType, out result))
                    return null;
                var specializedType = result.MakeGenericType(genericArguments);
                return specializedType;
            }
            if (!_solvedTypes.TryGetValue(type, out result))
                return null;
            return result;
        }
    }
}