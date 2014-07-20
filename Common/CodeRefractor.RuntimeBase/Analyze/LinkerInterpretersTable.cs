#region Usings

using System;
using System.Reflection;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.RuntimeBase;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Analyze
{
    public static class LinkerInterpretersTable
    {
        public static string WriteHeaderMethod(this MethodBase methodBase, ClosureEntities crRuntime, bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName();

            var sb = new StringBuilder();

            var arguments = methodBase.GetArgumentsAsText(pinvoke:methodBase.IsPinvoke());

            sb.AppendFormat("{0} {1}({2})",
                retType, methodBase.ClangMethodSignature(crRuntime), arguments);
            if (writeEndColon)
                sb.Append(";");

            sb.AppendLine();
            return sb.ToString();
        }

        public static string GetArgumentsAsText(this MethodBase method, bool pinvoke=false)
        {
            var parameterInfos = method.GetParameters();
            var arguments = String.Join(", ",
                CommonExtensions.GetParamAsPrettyList(parameterInfos,pinvoke));
            if (method.IsStatic) return arguments;
            var thisText = String.Format("const {0}& _this", method.DeclaringType.ToCppName());
            return parameterInfos.Length == 0
                ? thisText
                : String.Format("{0}, {1}", thisText, arguments);
        }
    }
}