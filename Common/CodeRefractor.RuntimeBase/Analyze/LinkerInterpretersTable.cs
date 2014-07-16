#region Usings

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.MiddleEnd;
using CodeRefractor.MiddleEnd.Interpreters;
using CodeRefractor.RuntimeBase.MiddleEnd;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public static class LinkerInterpretersTable
    {
        public static string WriteHeaderMethod(this MethodBase methodBase, ClosureEntities crRuntime, bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName();

            var genericTypeCount = methodBase.DeclaringType.GetGenericArguments().Length;

            var sb = new StringBuilder();

            var arguments = methodBase.GetArgumentsAsText();

            sb.AppendFormat("{0} {1}({2})",
                retType, methodBase.ClangMethodSignature(crRuntime), arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }

        public static string GetArgumentsAsText(this MethodBase method)
        {
            var parameterInfos = method.GetParameters();
            var arguments = String.Join(", ",
                CommonExtensions.GetParamAsPrettyList(parameterInfos));
            if (!method.IsStatic)
            {
                var thisText = String.Format("const {0}& _this", method.DeclaringType.ToCppName(true));
                return parameterInfos.Length == 0
                    ? thisText
                    : String.Format("{0}, {1}", thisText, arguments);
            }
            return arguments;
        }


        public static readonly Dictionary<string, MethodInterpreter> Methods =
            new Dictionary<string, MethodInterpreter>();

        public static readonly Dictionary<string, MethodBase> RuntimeMethods =
            new Dictionary<string, MethodBase>();

        public static void Clear()
        {
            Methods.Clear();
        }
    }
}