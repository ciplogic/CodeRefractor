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
        private const string ValidDelegatesCode = @"

#include <vector>

typedef void (*callBack)(int, int);

class DelegateVoid_Int32_Int32 {
	std::vector< std::function<void(int, int)> > functions;
public:
	void Invoke(int arg0, int arg1);
	void Register(std::function<void(int, int)>  );
};


CustomDelegateVoid_Int32_Int32 callbackDelegate1;

void RegisterCallbackDelegate1Void_Int32_Int32(){
	callbackDelegate1.Register(callBack);
}

void callBackToDelegate(int arg0,int arg1){
	callbackDelegate1.Invoke(arg0, arg1);
}


void methodToBeCalled(double customData, int arg0, int arg1){
	
}

struct CallerClosure{

	double customData;
	CallerClosure(double data) {
		customData = data;
	}
	void operator()(int arg0, int arg1) const {
		methodToBeCalled(customData, arg0, arg1);
	} 	
};


void RegisterMethodWithCustomData(){
	callbackDelegate1.Register(std::function<void (int, int)> () = CallerClosure(2.3) ); 
}

void CustomDelegateVoid_Int32_Int32::Invoke(int arg0, int arg1){
	for(auto it = functions.begin(); it!= functions.end(); ++it){
		(*it)(arg0, arg1);
	}
}

void CustomDelegateVoid_Int32_Int32::Register(std::function<void(int, int)> stdFn ){
	//std::function<void(int, int)> stdFn(fn);
	functions.push_back(stdFn);
}
";

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
                var parameters = delegateType.Value.GetMethodArgumentTypes().Skip(1).ToArray();

                var parametersFormat = string.Join(",", parameters.Select(paramType => paramType.ToCppMangling()));
                var typePrefixFormat = string.Join("_", parameters.Select(paramType => paramType.ToCppMangling()));
                var paramIndex = 0;
                var namedTypeArgs = string.Join(", ",
                    parameters.Select(
                        par => string.Format("{0} arg{1}", par.ToCppMangling(), paramIndex++)));

                paramIndex = 0;
                var namedArgs = string.Join(", ",
                    parameters.Select(
                        par => string.Format("arg{0}", paramIndex++)));
                
                sb.AppendFormat("typedef void(*callBack{0}) ({1});",
                    id, parametersFormat)
                    .AppendLine();
                sb.AppendFormat("struct CustomDelegate{0} {{", id)
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
                sb.AppendLine("for(auto it = _functions.begin(); it!= _functions.end(); ++it)");

                sb.AppendFormat("\t(*it)({0});",namedArgs)
                    .AppendLine();
                sb.AppendLine("}");

                sb.AppendLine("}; //end of class delegate");
                id++;
            }

            return sb.ToString();
        }

        public static bool IsTypeDelegate(Type declaringType)
        {
            return (Instance._delegateTypes.ContainsKey(declaringType));
        }
    }
}
