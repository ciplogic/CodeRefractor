#include "sloth.h"
#include <functional>
struct System_Object; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _Test; 
struct System_Object {
    int _typeId;
};

struct System_String : public System_Object {
    System_String() {
        typeId = 5;
    }

    std::shared_ptr< Array < System_Char > > Text;
};

struct _Test : public System_Object {
};


System_Void _Test_Main();

System_Int32 _Test_MessageBox(System_Int32 handle, System_Char* message, System_Char* title, System_UInt32 type);

#include "runtime_base.hpp"
// --- Begin definition of virtual implementingMethod tables ---
System_Void setupTypeTable();


///--- PInvoke code --- 
typedef System_Int32 ( *dll_method_1_type)(System_Int32 handle, System_Char* message, System_Char* title, System_UInt32 type);
dll_method_1_type dll_method_1;

System_Int32 _Test_MessageBox(System_Int32 handle, std::shared_ptr<System_String> message, std::shared_ptr<System_String> title, System_UInt32 type) {
    System_Char* _message = message.get()->Text.get()->Items;
    System_Char* _title = title.get()->Text.get()->Items;

    return dll_method_1(handle, _message, _title, type);
}

///---Begin closure code --- 
System_Void _Test_Main()
{
System_Int32 vreg_1;

vreg_1 = _Test_MessageBox(0, _str(0), _str(1), 0);
return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System_getArgumentsAsList(argc, argv);
initializeRuntime();
_Test_Main();
return 0;
}
System_Void mapLibs() {
auto lib_0 = LoadNativeLibrary(L"User32.dll");
dll_method_1 = (dll_method_1_type)LoadNativeMethod(lib_0, "MessageBox");
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
_AddJumpAndLength(0, 7);
_AddJumpAndLength(8, 5);
} // buildStringTable
const wchar_t _stringTable[14] = {
77, 101, 115, 115, 97, 103, 101, 0 /* "Message" */, 
84, 105, 116, 108, 101, 0 /* "Title" */
}; // _stringTable 

