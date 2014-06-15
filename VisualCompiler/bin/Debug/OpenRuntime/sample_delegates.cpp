#include "sloth.h"
struct _NBody; 
struct System_Object; 
struct _OnCall; 
struct System_Console; 
struct _NBody {
};
struct System_Console {
};


typedef void(*callBack0) (System_Int32);
struct CustomDelegate0 {
	std::vector< std::function<void(System_Int32)> > _functions;
void Register(std::function<void(System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate

;
struct _OnCall : CustomDelegate0 {
};

System_Void _NBody_Main();

System_Void _NBody_DoCall(System_Int32 data);

System_Void System_Console_WriteLine(System_Int32 value);

void _OnCall_ctor(std::shared_ptr<_OnCall> vreg_3, void*, std::function<void(int)> vreg_2){
	vreg_3->Register(vreg_2); 
}
void _OnCall_Invoke(std::shared_ptr<_OnCall> vreg_3, int arg0){
	vreg_3->Invoke(arg0);
}

#include "runtime_base.partcpp"
///---Begin closure code --- 
System_Void _NBody_Main()
{
std::function<void(int)> vreg_2;
std::shared_ptr<_OnCall> vreg_3;

vreg_2=_NBody_DoCall;
vreg_3 = std::make_shared<_OnCall >();
_OnCall_ctor(vreg_3, nullptr, vreg_2);
_OnCall_Invoke(vreg_3, 3);
return;
}


System_Void _NBody_DoCall(System_Int32 data)
{

System_Console_WriteLine(data);
return;
}


System_Void System_Console_WriteLine(System_Int32 value)
{

return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System_getArgumentsAsList(argc, argv);
initializeRuntime();
_NBody_Main();
return 0;
}
void mapLibs() {
}

void RuntimeHelpersBuildConstantTable() {
}

void buildStringTable() {
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 
