#region Uses

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.MiddleEnd.Interpreters.NonCil;

#endregion

namespace CodeRefractor.Runtime.Annotations
{
    public abstract class CrTypeResolver
    {
        readonly Dictionary<Type, Type> _mappedTypes = new Dictionary<Type, Type>();
        public abstract MethodInterpreter Resolve(MethodBase methodInterpreter);

        protected void MapType<T>(Type mappedType)
        {
            _mappedTypes[mappedType] = typeof (T);
        }

        protected static void ResolveAsPinvoke(PlatformInvokeMethod methodInterpreter, string libraryName,
            CallingConvention callingConvention = CallingConvention.Winapi)
        {
            var method = methodInterpreter.Method;

            methodInterpreter.LibraryName = libraryName;
            methodInterpreter.EntryPoint = method.Name;
            methodInterpreter.MethodName = method.Name;
            methodInterpreter.CallingConvention = callingConvention;
        }

        public virtual Type ResolveType(Type type)
        {
            Type result;
            return _mappedTypes.TryGetValue(type, out result) ? result : null;
        }
    }
}