#include "sloth.h"
#include <functional>
struct VbCrInput_Module1; 
struct System_IntPtr; 
struct System_String&; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};

System_Void VbCrInput_Module1_Main();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

///--- PInvoke code --- 
typedef System_Int32 ( *dll_method_1_type)(System_IntPtr hWnd, std::shared_ptr<System_String>*  lpText, std::shared_ptr<System_String>*  lpCaption, System_UInt32 uType);
dll_method_1_type dll_method_1;
System_Int32 VbCrInput_Module1_MessageBoxA(System_IntPtr hWnd, std::shared_ptr<System_String>*  lpText, std::shared_ptr<System_String>*  lpCaption, System_UInt32 uType)
{
return dll_method_1(hWnd, lpText, lpCaption, uType);
}

///---Begin closure code --- 
System_Void VbCrInput_Module1_Main()

{
std::shared_ptr<System_String> local_0;
std::shared_ptr<System_String> local_1;
System_IntPtr vreg_1;
std::shared_ptr<System_String>*  vreg_2;
std::shared_ptr<System_String>*  vreg_3;
System_Int32 vreg_4;

vreg_1 = 0;
local_0 = _str(0);
vreg_2 = &local_0;
local_1 = _str(1);
vreg_3 = &local_1;
vreg_4 = VbCrInput_Module1_MessageBoxA((System_IntPtr)vreg_1, vreg_2, vreg_3, 64);
return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System_getArgumentsAsList(argc, argv);
initializeRuntime();
VbCrInput_Module1_Main();
return 0;
}
System_Void mapLibs() {
auto lib_0 = LoadNativeLibrary(L"user32");
dll_method_1 = (dll_method_1_type)LoadNativeMethod(lib_0, "MessageBoxA");
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
_AddJumpAndLength(0, 13);
_AddJumpAndLength(14, 4);
} // buildStringTable
const wchar_t _stringTable[19] = {
72, 101, 108, 108, 111, 44, 32, 119, 111, 114, 108, 100, 33, 0 /* "Hello, world!" */, 
84, 101, 115, 116, 0 /* "Test" */
}; // _stringTable 

