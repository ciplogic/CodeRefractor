using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using CodeRefractor.RuntimeBase.DataBase;

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class DelegateManager
    {
        public static void RegisterType(Type declaringType, MethodInfo signature)
        {
            Instance._delegateTypes[declaringType] = signature;
        }
        public static DelegateManager Instance = new DelegateManager();
        readonly Dictionary<Type, MethodInfo> _delegateTypes =new Dictionary<Type, MethodInfo>();

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

        private static void GenerateDelegateCode(StringBuilder sb, int id, KeyValuePair<Type, MethodInfo> delegateType)
        {
            var parameters = delegateType.Value.GetMethodArgumentTypes().Skip(1).ToArray();

            var parametersFormat = TypeNamerUtils.GetCommaSeparatedParameters(parameters);
            var typePrefixFormat = string.Join("_", parameters.Select(paramType => paramType.ToCppMangling()));
            var paramIndex = 0;
            var namedTypeArgs = string.Join(", ",
                parameters.Select(
                    par => string.Format("{0} arg{1}", par.ToCppMangling(), paramIndex++)));

            paramIndex = 0;
            var namedArgs = string.Join(", ",
                parameters.Select(
                    par => string.Format("arg{0}", paramIndex++)));

           /* sb.AppendFormat("typedef void(*callBack{0}) ({1});",
                id, parametersFormat)
                .AppendLine();
            * */
            sb.AppendFormat("struct {0} : System_Object {{", delegateType.Key.ToCppMangling())
                .AppendLine();

            sb.AppendFormat("	std::vector< std::function<void({0})> > _functions;", parametersFormat)
                .AppendLine();

            sb.AppendFormat("void Register(std::function<void({0}) > fn) {{", parametersFormat)
                .AppendLine();
            sb.AppendFormat("_functions.push_back(fn);")
                .AppendLine()
                .AppendLine("}");

            sb.AppendFormat("void Invoke(");

            sb.Append(namedTypeArgs);
            sb.AppendLine(") {");
            sb.AppendLine("for(auto it : _functions)");

            sb.AppendFormat("\t(it)({0});", namedArgs)
                .AppendLine();
            sb.AppendLine("}");

            sb.AppendLine("}; //end of class delegate");

            sb.AppendFormat("void {0}_ctor(const std::shared_ptr<{0}>& _delegate, void*, std::function<void({1})> fn){{",
                    delegateType.Key.ToCppMangling(),
                    parametersFormat)
              .AppendLine();

            sb.AppendLine("  _delegate->Register(fn);");
            sb.AppendLine("}");
            if (!String.IsNullOrEmpty(namedTypeArgs))
            {
                sb.AppendFormat("void {0}_Invoke(const std::shared_ptr<{0}>& _delegate, {1}){{",
                    delegateType.Key.ToCppMangling(),
                    namedTypeArgs)
                    .AppendLine();
            }
            else
            {
                sb.AppendFormat("void {0}_Invoke(const std::shared_ptr<{0}>& _delegate){{",
                    delegateType.Key.ToCppMangling(),
                    namedTypeArgs)
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
