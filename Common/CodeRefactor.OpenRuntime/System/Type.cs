using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using CodeRefactor.OpenRuntime.System.Reflection;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime.System
{
 
    [ExtensionsImplementation(typeof(Type))]
    public abstract class Type : MemberInfo
    {

        public static readonly Type[] EmptyTypes = new Type[0];

        [MapMethod(IsStatic = false)]
        public static Type GetTypeFromHandle(RuntimeTypeHandle handle)
        {
            return null;
        }
        
        public abstract Type BaseType
        {
            [MapMethod(IsStatic = false)]
            get;
        }

        public abstract bool IsEnum
        {
            [MapMethod(IsStatic = false)]
            get;
        }

        public abstract string Namespace
        {
            [MapMethod(IsStatic = false)]
            get;
        }

        public abstract string FullName
        {
            [MapMethod(IsStatic = false)]
            get;
        }

        public abstract bool IsGenericType
        {
            [MapMethod(IsStatic = false)]
            get;
        }

        public abstract Type GetGenericTypeDefinition();

        public abstract Type[] GetGenericArguments();

        public bool IsValueType
        {
            [MapMethod(IsStatic = false)] get { return false; }
        }

        [MapMethod(IsStatic = false)]
        public override string ToString()
        {
            return this.FullName;
        }
    }
}
