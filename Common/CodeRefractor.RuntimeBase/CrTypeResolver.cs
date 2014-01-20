using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.Methods;

namespace CodeRefractor.RuntimeBase
{
    public abstract class CrTypeResolver
    {
        private readonly Dictionary<Type, Type> _mappedTypes = new Dictionary<Type, Type>(); 
        public abstract bool Resolve(MethodInterpreter methodInterpreter);

        protected void MapType<T>(Type mappedType)
        {
            _mappedTypes[mappedType] = typeof(T);
        }

        public abstract Type ResolveType(Type type);

        protected static void ResolveAsPinvoke(MethodInterpreter methodInterpreter, string libraryName,
            CallingConvention callingConvention = CallingConvention.Winapi)
        {
            var method = methodInterpreter.Method;

            methodInterpreter.Kind = MethodKind.PlatformInvoke;
            methodInterpreter.PlatformInvoke.LibraryName = libraryName;
            methodInterpreter.PlatformInvoke.EntryPoint = method.Name;
            methodInterpreter.Description.MethodName = method.Name;
            methodInterpreter.Description.CallingConvention = callingConvention;
        }

        protected Type MappedResolveType(Type type)
        {
            Type result;
            return _mappedTypes.TryGetValue(type, out result) ? result : null;
        }
    }
}