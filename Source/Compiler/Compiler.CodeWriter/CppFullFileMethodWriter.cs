#region Usings

using System;
using System.Reflection;
using System.Text;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Runtime;
using Compiler.CodeWriter.Linker;

#endregion

namespace Compiler.CodeWriter
{
    public static class CppFullFileMethodWriter
    {
        public static string WriteHeaderMethodWithEscaping(this MethodBase methodBase, bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName(true);

            var sb = new StringBuilder();
            var declaringType = methodBase.DeclaringType;
            if (declaringType.IsGenericType)
            {
                var genericTypeCount = declaringType.GetGenericArguments().Length;

                if (genericTypeCount > 0)
                    sb.AppendLine(genericTypeCount.GetTypeTemplatePrefix());
            }

            var arguments = methodBase.GetArgumentsAsTextWithEscaping();

            sb.AppendFormat("{0} {1}({2})",
                retType, methodBase.ClangMethodSignature(), arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }

        public static string GetMethodDescriptor(this MethodBase method)
        {
            return CrRuntimeLibrary.GetMethodDescription(method);
        }

        public static string GetArgumentsAsTextWithEscaping(this MethodBase method)
        {
            var parameterInfos = method.GetParameters();
            var escapingBools = BuildEscapingBools(method);
            var sb = new StringBuilder();
            var index = 0;
            if (!method.IsStatic)
            {
                var argumentTypeDescription = UsedTypeList.Set(method.DeclaringType.GetMappedType());
                var thisText = String.Format("const {0}& _this", argumentTypeDescription.ClrType.ToCppName(true));
                if (!escapingBools[0])
                {
                    thisText = String.Format("{0} _this",
                        argumentTypeDescription.ClrType.ToCppName(true, EscapingMode.Pointer));
                }
                sb.Append(thisText);
                index++;
            }
            var isFirst = index == 0;

            for (index = 0; index < parameterInfos.Length; index++)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    sb.Append(", ");
                }
                var parameterInfo = parameterInfos[index];
                var isSmartPtr = escapingBools[index];
                var nonEscapingMode = isSmartPtr ? EscapingMode.Smart : EscapingMode.Pointer;
                var argumentTypeDescription = UsedTypeList.Set(parameterInfo.ParameterType.GetMappedType());
                sb.AppendFormat("{0} {1}",
                    argumentTypeDescription.ClrType.ToCppName(true, isSmartPtr: nonEscapingMode),
                    parameterInfo.Name);
            }
            return sb.ToString();
        }

        public static bool[] BuildEscapingBools(this MethodBase method)
        {
            var parameters = method.GetParameters();
            var escapingBools = new bool[parameters.Length + 1];

            var escapeData = method.EscapingParameterData();
            if (escapeData != null)
            {
                foreach (var escaping in escapeData)
                {
                    if (escaping.Value)
                        escapingBools[escaping.Key] = true;
                }
            }
            else
            {
                for (var index = 0; index <= parameters.Length; index++)
                {
                    escapingBools[index] = true;
                }
            }
            return escapingBools;
        }
    }
}