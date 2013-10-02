﻿#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeRefractor.CompilerBackend.Optimizations.EscapeAndLowering;
using CodeRefractor.RuntimeBase;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Shared;

#endregion

namespace CodeRefractor.CompilerBackend.OuputCodeWriter
{
    public static class CppFullFileMethodWriter
    {
        public static MetaLinker CreateLinkerFromEntryPoint(this MethodInfo definition)
        {
            var linker = new MetaLinker();
            linker.SetEntryPoint(definition);
            MetaLinker.ComputeDependencies(definition);
            linker.Interpret();
            MetaLinkerOptimizer.OptimizeMethods();
            return linker;
        }

        public static string WriteHeaderMethod(this MethodBase methodBase, bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName();

            var sb = new StringBuilder();
            var arguments = methodBase.GetArgumentsAsText();

            sb.AppendFormat("{0} {1}({2})",
                            retType, methodBase.ClangMethodSignature(), arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }


        public static string WriteHeaderMethodWithEscaping(this MethodBase methodBase, bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName();

            var sb = new StringBuilder();
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
            var declaringType = method.DeclaringType;
            var mappedtypeAttr = declaringType.GetCustomAttribute<MapTypeAttribute>();
            var mappedType2 = mappedtypeAttr == null ? null : mappedtypeAttr.MappedType;
            var format = String.Format("{0}.{1}({2})",
                                       mappedType2 == null ? declaringType.FullName : mappedType2.FullName, method.Name,
                                       method.GetArgumentsAsText());
            return format;
        }
        public static string GetArgumentsAsText(this MethodBase method)
        {
            var parameterInfos = method.GetParameters();
            var arguments = String.Join(", ",
                                        CommonExtensions.GetParamAsPrettyList(parameterInfos));
            if (!method.IsStatic)
            {
                var thisText = String.Format("const {0}& _this", method.DeclaringType.GetMappedType().ToCppName());
                return parameterInfos.Length == 0
                           ? thisText
                           : String.Format("{0}, {1}", thisText, arguments);
            }
            return arguments;
        }
        public static string GetArgumentsAsTextWithEscaping(this MethodBase method)
        {
            var parameterInfos = method.GetParameters();
            var escapingBools = BuildEscapingBools(method, parameterInfos);
            var sb = new StringBuilder();
            var index = 0;
            if (!method.IsStatic)
            {
                var thisText = String.Format("const {0}& _this", method.DeclaringType.GetMappedType().ToCppName());
                if(!escapingBools[0])
                {
                    thisText = String.Format("{0} _this", method.DeclaringType.GetMappedType().ToCppName(NonEscapingMode.Pointer));
                }
                sb.Append(thisText);
                index++;
            }
            bool isFirst = index==0;
            
            for (index=0; index < parameterInfos.Length; index++)
            {
                if (isFirst)
                    isFirst = false;
                else
                {
                    sb.Append(", ");
                }
                var parameterInfo = parameterInfos[index];
                var isSmartPtr = escapingBools[index];
                var nonEscapingMode = isSmartPtr ? NonEscapingMode.Smart : NonEscapingMode.Pointer;
                sb.AppendFormat("{0} {1}", 
                    parameterInfo.ParameterType.GetMappedType().ToCppName(nonEscapingMode ), 
                    parameterInfo.Name);
            }
            return sb.ToString();
        }

        public static bool[] BuildEscapingBools(MethodBase method, ParameterInfo[] parameterInfos)
        {
            var escapingBools = new bool[parameterInfos.Length+1];
            var escapeData = AnalyzeParametersAreEscaping.EscapingParameterData(method);
            if (escapeData != null)
                foreach (var escaping in escapeData)
                {
                    if (escaping.Value)
                        escapingBools[escaping.Key] = true;
                }
            return escapingBools;
        }
    }
}