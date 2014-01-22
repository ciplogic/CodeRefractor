using System;
using System.Collections.Generic;
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

        protected static void ResolveAsPinvoke(MethodInterpreter methodInterpreter, string libraryName,
            CallingConvention callingConvention = CallingConvention.Winapi)
        {
            var method = methodInterpreter.Method;

            methodInterpreter.Kind = MethodKind.PlatformInvoke;
            methodInterpreter.Description.LibraryName = libraryName;
            methodInterpreter.Description.EntryPoint = method.Name;
            methodInterpreter.Description.MethodName = method.Name;
            methodInterpreter.Description.CallingConvention = callingConvention;
        }

        public virtual Type ResolveType(Type type)
        {
            Type result;
            return _mappedTypes.TryGetValue(type, out result) ? result : null;
        }
    }
}