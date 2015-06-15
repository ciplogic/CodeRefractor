#region Uses

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.Util;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class DelegateManager
    {
        public static DelegateManager Instance = new DelegateManager();
        readonly Dictionary<Type, MethodInfo> _delegateTypes = new Dictionary<Type, MethodInfo>();

        public static void RegisterType(Type declaringType, MethodInfo signature)
        {
            Instance._delegateTypes[declaringType] = signature;
        }

        public string BuildDelegateContent()
        {
            var sb = new StringBuilder();
            var id = 0;
            foreach (var delegateType in _delegateTypes)
            {
                GenerateDelegateCode(sb, id, delegateType);
                id++;
            }

            return sb.ToString();
        }

        static void GenerateDelegateCode(StringBuilder sb, int id, KeyValuePair<Type, MethodInfo> delegateType)
        {
            var parameters = delegateType.Value.GetMethodArgumentTypes().Skip(1).ToArray();

            var parametersFormat = TypeNamerUtils.GetCommaSeparatedParameters(parameters);
            var typePrefixFormat = string.Join("_", parameters.Select(paramType => paramType.ToCppMangling()));
            int[] paramIndex = { 0 };
            var namedTypeArgs = string.Join(", ",
                parameters.Select(
                    par => $"{par.ToCppMangling()} arg{paramIndex[0]++}"));

            paramIndex[0] = 0;
            var namedArgs = string.Join(", ",
                parameters.Select(
                    par => $"arg{paramIndex[0]++}"));

            /* sb.AppendFormat("typedef void(*callBack{0}) ({1});",
                id, parametersFormat)
                .AppendLine();
            * */
            sb.AppendFormat("struct {0} : System_Object {{", delegateType.Key.ToCppMangling())
                .AppendLine();

            sb.AppendFormat("	std::vector< std::function<System_Void({0})> > _functions;", parametersFormat)
                .AppendLine();

            sb.AppendFormat("System_Void Register(std::function<System_Void({0}) > fn) {{", parametersFormat)
                .AppendLine();
            sb.AppendFormat("_functions.push_back(fn);")
                .AppendLine()
                .AppendLine("}");

            sb.AppendFormat("System_Void Invoke(");

            sb.Append(namedTypeArgs);
            sb.AppendLine(") {");
            sb.AppendLine("for(auto it : _functions)");

            sb.AppendFormat("\t(it)({0});", namedArgs)
                .AppendLine();
            sb.AppendLine("}");

            sb.AppendLine("}; //end of class delegate");


            sb.AppendFormat(
                "System_Void {0}_ctor(const {2}<{0}>& _delegate, System_Void*, std::function<System_Void({1})> fn){{",
                delegateType.Key.ToCppMangling(),
                parametersFormat,
                TypeNamerUtils.StdSharedPtr)
                .AppendLine();

            sb.AppendLine("  _delegate->Register(fn);");
            sb.AppendLine("}");
            if (!string.IsNullOrEmpty(namedTypeArgs))
            {
                sb.AppendFormat("System_Void {0}_Invoke(const {2}<{0}>& _delegate, {1}){{",
                    delegateType.Key.ToCppMangling(),
                    namedTypeArgs,
                    TypeNamerUtils.StdSharedPtr)
                    .AppendLine();
            }
            else
            {
                sb.AppendFormat("System_Void {0}_Invoke(const {1}<{0}>& _delegate){{",
                    delegateType.Key.ToCppMangling(),
                    TypeNamerUtils.StdSharedPtr)
                    .AppendLine();
            }

            sb.AppendFormat("  _delegate->Invoke({0});", namedArgs)
                .AppendLine();
            sb.AppendLine("}");
        }

        public static bool IsTypeDelegate(Type declaringType)
        {
            return (Instance._delegateTypes.ContainsKey(declaringType));
        }
    }
}