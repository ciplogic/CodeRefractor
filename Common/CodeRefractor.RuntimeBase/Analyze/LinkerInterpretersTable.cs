#region Uses

using System.Reflection;
using System.Text;
using CodeRefractor.ClosureCompute;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.Analyze
{
    public static class LinkerInterpretersTable
    {
        public static string WriteHeaderMethod(this MethodBase methodBase, ClosureEntities crRuntime,
            bool writeEndColon = true)
        {
            var retType = methodBase.GetReturnType().ToCppName();

            var sb = new StringBuilder();

            // Since the method signature for the call is the external interface we
            // provide to our clients, the signature remains the same, an dinternally the
            // PInvoke method will do parameter marshalling in case the types are different.
            var arguments = methodBase.GetArgumentsAsText();

            sb.AppendFormat("{0} {1}({2})",
                retType, methodBase.ClangMethodSignature(crRuntime), arguments);

            if (writeEndColon)
                sb.Append(";");

            return sb.ToString();
        }

        public static string GetArgumentsAsText(this MethodBase method, bool pinvoke = false)
        {
            var parameterInfos = method.GetParameters();
            var arguments = string.Join(", ",
                CommonExtensions.GetParamAsPrettyList(parameterInfos, pinvoke));
            if (method.IsStatic) return arguments;
            var thisText = string.Format("const {0}& _this", method.DeclaringType.ToCppName());
            return parameterInfos.Length == 0
                ? thisText
                : string.Format("{0}, {1}", thisText, arguments);
        }
    }
}