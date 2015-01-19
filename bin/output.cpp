#include "sloth.h"
#include <functional>
struct System_Object; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _Test; 
struct _MyClass; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct System_Object {
int _typeId;
};
struct System_String : public System_Object {
System_String() {_typeId = 6; }
 std::shared_ptr< Array < System_Char > > Text;
};
struct _Test : public System_Object {
};
struct _MyClass : public System_Object {
};
struct System_Console : public System_Object {
};

System_Void _Test_Main();

System_Void _MyClass_Display(const std::shared_ptr<_MyClass>& _this, System_Int32 value);

System_Void System_Console_WriteLine(System_Int32 value);

#include "runtime_base.hpp"
// --- Begin definition of virtual implementingMethod tables ---
System_Void setupTypeTable();


#include "stdio.h"
System_Void System_Console_WriteLine(System_Int32 value)
{
printf("%d\n", value);
}
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Test_Main()
{
std::shared_ptr<_MyClass> vreg_1;

vreg_1 = std::make_shared<_MyClass >();
vreg_1->_typeId = 3;
_MyClass_Display(vreg_1, 3);
return;
}


System_Void _MyClass_Display(const std::shared_ptr<_MyClass>& _this, System_Int32 value)
{
System_Int32 vreg_1;

vreg_1 = value+3;
System_Console_WriteLine(vreg_1);
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
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

