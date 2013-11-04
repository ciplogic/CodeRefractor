using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Runtime;

namespace CodeRefractor.RuntimeBase
{
    public static class TypeNamerUtils
    {
        public const string StdSharedPtr = "std::shared_ptr";

        public static string ClangMethodSignature(this MethodBase method)
        {
            var mappedType = CrRuntimeLibrary.Instance.GetReverseType(method.DeclaringType);
            var declaringType = mappedType ?? method.DeclaringType;
            var typeName = declaringType.ToCppMangling();
            var methodName = method.Name;
            if (method is ConstructorInfo)
                methodName = "ctor";
            return String.Format("{0}_{1}", typeName, methodName);
        }


        public static string ToCppMangling(this Type type)
        {
            return IsVoid(type)
                       ? "void"
                       : type.Name.ToCppMangling(type.Namespace, 
                            type.IsGenericType
                            ? type.GetGenericArguments().ToList()
                            : null);
        }

        public static string ToCppMangling(this string s, string nameSpace = "", List<Type> classSpecializationType = null)
        {
            if (String.IsNullOrEmpty(s))
                return String.Empty;
            nameSpace = nameSpace ?? "";
            nameSpace = nameSpace.Replace(".", "_");
            if (s.Contains("`"))
            {
                s = s.Remove(s.IndexOf('`'));
            }
            if (classSpecializationType != null && classSpecializationType.Count > 0)
            {
                s = s + "_" + String.Join("_", classSpecializationType.Select(t => t.ToCppMangling()));

            }
            var fullName = nameSpace + "_" + s;
            if (s.EndsWith("[]"))
            {
                s = s.Remove(s.Length - 2, 2);
                fullName = String.Format("std::shared_ptr <Array<{0}_{1}> > &", nameSpace, s);
            }
            return fullName;
        }
        public static string ToCppName(this Type type, NonEscapingMode isSmartPtr = NonEscapingMode.Smart)
        {
            if (type == null)
                return "void*";
            type = type.ReversedType();
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var fullTypeName = elementType.ToCppName();
                switch (isSmartPtr)
                {
                    case NonEscapingMode.Smart:
                        return String.Format(StdSharedPtr+"< Array < {0} > >", fullTypeName);
                    case NonEscapingMode.Pointer:
                        return String.Format("Array < {0} > *", fullTypeName);
                    case NonEscapingMode.Stack:
                        return String.Format("Array < {0} > ", fullTypeName);
                }

            }
            if (type.IsClass || isSmartPtr != NonEscapingMode.Smart)
            {
                if (type.IsPrimitive || type.IsValueType)
                    isSmartPtr = NonEscapingMode.Stack;
                if (type.Name.EndsWith("*") || type.Name.EndsWith("&"))
                {
                    var elementType = type.GetElementType();
                    return String.Format("{0}* ", elementType.ToCppMangling());
                }
                switch (isSmartPtr)
                {
                    case NonEscapingMode.Smart:
                        return String.Format(StdSharedPtr+"<{0}>", type.ToCppMangling());
                    case NonEscapingMode.Pointer:
                        return String.Format("{0} *", type.ToCppMangling());
                    case NonEscapingMode.Stack:
                        return String.Format("{0} ", type.ToCppMangling());
                }
            }
            if (!type.IsClass || isSmartPtr != NonEscapingMode.Smart)
            {
                return type.IsSubclassOf(typeof(Enum))
                           ? "int"
                           : type.Name.ToCppMangling(type.Namespace);
            }
            if (type.IsByRef)
            {
                var elementType = type.GetElementType();
                return String.Format("{0}*", elementType.ToCppMangling());
            }
            return String.Format(StdSharedPtr+"<{0}>", type.ToCppMangling());
        }

        public static bool IsVoid(this Type type)
        {
            return type == typeof(void);
        }
    }
}