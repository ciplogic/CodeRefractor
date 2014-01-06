using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using CodeRefractor.RuntimeBase.FrontEnd;
using CodeRefractor.RuntimeBase.MiddleEnd;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public static class LinkerInterpretersTable
    {
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

        
        public static Dictionary<string, MethodInterpreter> Methods =
            new Dictionary<string, MethodInterpreter>();
        public static Dictionary<string, MethodBase> RuntimeMethods =
            new Dictionary<string, MethodBase>();
        public static void Clear()
        {
            Methods.Clear();
        }
        public static void Register(MethodInterpreter method)
        {
            var methodName = method.Method.WriteHeaderMethod(false);
            GlobalMethodPoolUtils.Register(method);
            Methods[methodName] = method;
        }

        public static MethodInterpreter Register(MethodBase method)
        {
            var interpreter = method.GetRegisteredInterpreter();
            if (interpreter != null)
                return interpreter;
            interpreter = new MethodInterpreter(method);
            Register(interpreter);
            return interpreter;
        }
        public static void RegisterRuntimeMethod(KeyValuePair<string, MethodBase> usedMethod)
        {
            RuntimeMethods[usedMethod.Key] = usedMethod.Value;
        }
    }
}
