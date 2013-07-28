#region Usings

using System;
using System.Linq;
using System.Reflection;
using System.Text;

#endregion

namespace ClassOpenRuntimeCodeGenerator
{
    internal class CodeBuilder
    {
        private readonly Type _sourceType;
        private readonly string _destinationType;
        private readonly Form1ViewModelData _viewModel;

        public CodeBuilder(Type sourceType, string destinationType, Form1ViewModelData viewModel)
        {
            _sourceType = sourceType;
            _destinationType = destinationType;
            _viewModel = viewModel;
        }

        public string GenerateSource()
        {
            var sb = new StringBuilder();


            var methods = _sourceType.GetMethods(BindingFlags.Static | BindingFlags.Public);
            foreach (var methodInfo in methods)
            {
                BuildPrefixSignature(_viewModel, sb);
                BuildMethodSignature(sb, methodInfo);
            }

            return sb.ToString();
        }

        private static void BuildPrefixSignature(Form1ViewModelData viewModel, StringBuilder sb)
        {
            if (viewModel.IsPure)
                sb.AppendLine("[PureMethod]");

            if (viewModel.IsCppMethod)
            {
                sb.AppendFormat("[CppMethodBody(Header = \"{0}\", Code = \"\")]", viewModel.Header);
                sb.AppendLine();
            }
        }


        private static void BuildMethodSignature(StringBuilder sb, MethodInfo methodInfo)
        {
            sb.Append("public static ");
            var isVoid = methodInfo.ReturnType == typeof(void);
            sb.Append(isVoid ? "void" : methodInfo.ReturnType.FullName);
            sb.AppendFormat("{0} (", methodInfo.Name);
            var methodParameters = methodInfo.GetParameters();
            var parameterList = methodParameters.Select(
                p => string.Format("{0} {1}",
                    p.ParameterType.FullName, p.Name)
                ).ToList();
            sb.Append(string.Join(", ", parameterList));
            sb.AppendLine(")");
            sb.AppendLine("{");
            if (!isVoid)
                sb.AppendLine("\treturn null;");
            sb.AppendLine("}");
        }
    }
}