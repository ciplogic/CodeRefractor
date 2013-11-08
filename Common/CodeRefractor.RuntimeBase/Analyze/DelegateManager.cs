using System;
using System.Collections.Generic;
using System.Reflection;
using CodeRefractor.RuntimeBase.DataBase;

namespace CodeRefractor.RuntimeBase.Analyze
{
    class DelegateManager
    {
        private const string ValidDelegatesCode = @"

#include <vector>

void (*callBack)(int, int);

class CustomDelegateVoid_Int32_Int32 {
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

        public static bool IsTypeDelegate(Type declaringType)
        {
            return (Instance._delegateTypes.ContainsKey(declaringType));
        }
    }
}
