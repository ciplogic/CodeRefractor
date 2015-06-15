#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.Interpreters;

#endregion

namespace CodeRefractor.Util
{
    /// <summary>
    ///     helper class for translating C# names to
    ///     C++ names
    /// </summary>
    public static class TypeNamerUtils
    {
        public static string StdSharedPtr = "std::shared_ptr";
        public static string SmartPtrHeader = "";
        /*
        public static string StdSharedPtr = "boost::shared_ptr";
        public static string SmartPtrHeader = "<boost>";
        */

        public static string GetMethodName(this MethodBase methodInfo)
        {
            if (methodInfo.DeclaringType != null && methodInfo.DeclaringType.IsInterface)
            {
                return methodInfo.DeclaringType.FullName.Replace("+", ".") + "." + methodInfo.Name;
            }
            return methodInfo.Name;
        }

        public static string GetMethodFullName(this MethodBase methodInfo)
        {
            if (methodInfo.DeclaringType != null)
                return methodInfo.DeclaringType.FullName + "." + methodInfo.Name;
            return methodInfo.Name;
        }

        public static List<Type> ImplementorsOfT(this Type t, ClosureEntities entities)
        {
            IEnumerable<Type> usedTypes =
                entities.MappedTypes.Values.Union(
                    entities.MappedTypes.Values.Where(g => g != g.GetReversedMappedType(entities))
                        .Select(g => g.GetReversedMappedType(entities))).ToList();

            var implementors = usedTypes.Where(y => t.IsAssignableFrom(y));
                // t.Assembly.GetTypes().Where(y=>t.IsAssignableFrom(y));// && t!=y);

            return implementors.ToList();
        }

        public static List<Type> ImplementorsOfT(this Type t, IEnumerable<Type> usedTypes)
        {
            var implementors = usedTypes.Where(y => t.IsAssignableFrom(y));

            return implementors.ToList();
        }

        public static string ClangMethodSignature(this MethodInterpreter method, ClosureEntities crRuntime)
        {
            var declaringType = method.OverrideDeclaringType ?? method.Method.DeclaringType;
            declaringType = declaringType.GetReversedMappedType(crRuntime);
            var sb = new StringBuilder();
            sb.Append(declaringType.ToCppMangling());
            sb.Append("_");
            var info = method.Method;
            var methodName = info.Name;
            if (info is ConstructorInfo)
                methodName = "ctor";
            //TODO: C++ does not expect names with angle brackets
            methodName = methodName
                .Replace("<", "_")
                .Replace(">", "_").
                Replace(".", "_");

            sb.Append(methodName);
            return sb.ToString();
        }

        public static string ClangMethodSignature(this MethodBase method, ClosureEntities crRuntime,
            Type declaringType = null)
        {
            var mappedType = declaringType ?? method.DeclaringType.GetReversedMappedType(crRuntime);

            var typeName = mappedType.ToCppMangling();

            var methodName = method.Name;
            if (method is ConstructorInfo)
                methodName = "ctor";
            methodName = methodName.Replace("<", "_").Replace(">", "_").Replace(".", "_");
                //TODO: C++ does not expect names with angle brackets
            return string.Format("{0}_{1}", typeName, methodName);
        }

        public static Type[] GetMethodArgumentTypes(this MethodBase method)
        {
            var resultList = new List<Type>();
            if (!method.IsStatic)
            {
                resultList.Add(method.DeclaringType);
            }
            var arguments = method.GetParameters();
            resultList.AddRange(arguments.Select(arg => arg.ParameterType));

            return resultList.ToArray();
        }

        public static string GetCommaSeparatedParameters(Type[] parameters)
        {
            return string.Join(",", parameters.Select(paramType => paramType.ToCppMangling()));
        }

        public static string ToCppMangling(this Type type)
        {
            var name = type.Name.Replace("<>", "__");
            name = name.Replace("`", "_");
            var genericArguments = type.GetGenericArguments();
            var genericTypes = string.Join("_", genericArguments.Select(tp => tp.ToCppMangling()));
            name += genericTypes;
            if (IsVoid(type)) return "System_Void";
            return name.ToCppMangling(type.Namespace);
        }

        public static bool IsGenericFieldUsage(this Type type)
        {
            return type.ContainsGenericParameters && !type.IsGenericTypeDefinition;
        }

        public static string ToCppMangling(this string s, string nameSpace = "")
        {
            if (string.IsNullOrEmpty(s))
                return string.Empty;
            nameSpace = nameSpace ?? "";
            nameSpace = nameSpace.Replace(".", "_");
            if (s.Contains("`"))
            {
                s = s.Remove(s.IndexOf('`'));
            }
            var fullName = nameSpace + "_" + s;
            if (s.EndsWith("[]"))
            {
                s = s.Remove(s.Length - 2, 2);
                fullName = string.Format("{2} <Array<{0}_{1}> > &", nameSpace, s, StdSharedPtr);
            }
            return fullName;
        }

        public static string ToDeclaredVariableType(this Type type,
            EscapingMode isSmartPtr = EscapingMode.Smart)
        {
            if (type == null)
                return "System_Void*";
            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var fullTypeName = elementType.ToCppName();
                switch (isSmartPtr)
                {
                    case EscapingMode.Smart:
                        return string.Format(StdSharedPtr + "< Array < {0} > >", fullTypeName);
                    case EscapingMode.Pointer:
                        //TODO: add the text to pointers to make possible 
                        // auto-vectorisation in gcc
                        // ' __restrict__ __attribute__ ((aligned (16))) '
                        return string.Format("Array < {0} >  *", fullTypeName);
                    case EscapingMode.Stack:
                        return string.Format("Array < {0} > ", fullTypeName);
                }
            }
            if ((type.IsClass || type.IsInterface) || isSmartPtr != EscapingMode.Smart)
            {
                if (type.IsPrimitive || type.IsValueType)
                    isSmartPtr = EscapingMode.Stack;
                if (type.Name.EndsWith("*") || type.Name.EndsWith("&"))
                {
                    var elementType = type.GetElementType();
                    var elementTypeCppName = elementType.ToCppName();
                    return string.Format("{0}* ", elementTypeCppName);
                }
                /*
                var typeParameters = type.GetGenericArguments();
                if (typeParameters.Length != 0)
                {
                    var genericSpecializations = typeParameters.Select(
                        t => t.ToCppMangling()
                        ).ToList();
                    var genericSpecializationString = string.Join(", ", genericSpecializations);

                    switch (isSmartPtr)
                    {
                        case EscapingMode.Smart:
                            return String.Format(StdSharedPtr + "<{0} <{1}> >", type.ToCppMangling(),
                                genericSpecializationString);
                        case EscapingMode.Pointer:
                            return String.Format("{0} <{1}>*", type.ToCppMangling(), genericSpecializationString);
                        case EscapingMode.Stack:
                            return String.Format("{0} <{1}>", type.ToCppMangling(), genericSpecializationString);
                    }
                }
                 * */
                switch (isSmartPtr)
                {
                    case EscapingMode.Smart:
                        return $"{StdSharedPtr}<{type.ToCppMangling()}>";
                    case EscapingMode.Pointer:
                        return $"{type.ToCppMangling()} *";
                    case EscapingMode.Stack:
                        return $"{type.ToCppMangling()} ";
                }
            }
            if (!(type.IsClass || type.IsInterface) || isSmartPtr != EscapingMode.Smart)
            {
                return type.IsSubclassOf(typeof (Enum))
                    ? "int"
                    : type.Name.ToCppMangling(type.Namespace);
            }
            if (type.IsByRef)
            {
                var elementType = type.GetElementType();
                return $"{elementType.ToCppMangling()}*";
            }
            return $"{StdSharedPtr}<{type.ToCppMangling()}>";
        }

        public static string ToCppName(this Type type,
            EscapingMode isSmartPtr = EscapingMode.Smart, bool isPInvoke = false)
        {
            if (type == null)
                return "System_Void*";

            if (type == typeof (string) && isPInvoke)
            {
                return "System_Char*";
            }

            if (type.IsArray)
            {
                var elementType = type.GetElementType();
                var fullTypeName = elementType.ToCppName();
                switch (isSmartPtr)
                {
                    case EscapingMode.Smart:
                        return $"{StdSharedPtr}< Array < {fullTypeName} > >";
                    case EscapingMode.Pointer:
                        return $"Array < {fullTypeName} > *";
                    case EscapingMode.Stack:
                        return $"Array < {fullTypeName} > ";
                }
            }
            if ((type.IsClass || type.IsInterface) || isSmartPtr != EscapingMode.Smart)
            {
                if (type.IsPrimitive || type.IsValueType)
                    isSmartPtr = EscapingMode.Stack;
                if (type.Name.EndsWith("*") || type.Name.EndsWith("&"))
                {
                    var elementType = type.GetElementType();
                    var elementTypeCppName = elementType.ToCppName();
                    return $"{elementTypeCppName}* ";
                }
                if (type.IsGenericFieldUsage())
                    isSmartPtr = EscapingMode.Stack;
                switch (isSmartPtr)
                {
                    case EscapingMode.Smart:
                        return string.Format(StdSharedPtr + "<{0}>", type.ToCppMangling());
                    case EscapingMode.Pointer:
                        return $"{type.ToCppMangling()}*";
                    case EscapingMode.Stack:
                        return $"{type.ToCppMangling()}";
                }
            }
            if (!(type.IsClass || type.IsInterface) || isSmartPtr != EscapingMode.Smart)
            {
                return type.IsSubclassOf(typeof (Enum))
                    ? "int"
                    : type.Name.ToCppMangling(type.Namespace);
            }
            if (type.IsByRef)
            {
                var elementType = type.GetElementType();
                return string.Format("{0}*", elementType.ToCppMangling());
            }

            return string.Format(StdSharedPtr + "<{0}>", type.ToCppMangling());
        }

        public static string GetTypeTemplatePrefix(this int genericTypeCount)
        {
            var typeList = new List<string>();
            for (var i = 1; i <= genericTypeCount; i++)
            {
                typeList.Add($"class T{i}");
            }
            var result = string.Join(", ", typeList);
            return $"template <{result}> ";
        }

        public static bool IsVoid(this Type type)
        {
            return type == typeof (void);
        }
    }
}