#include "sloth.h"
#include <functional>
struct System_Object; 
struct _A; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct _B; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _Test; 
struct System_Object {
int _typeId;
};
struct _A : public System_Object {
};
struct System_Console : public System_Object {
};
struct _B : public _A {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct _Test : public System_Object {
};

System_Void _Test_Main();

System_Void _B_F();

System_Void _A_F();

System_Void _B_G();

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

System_Void _A_F_vcall(const std::shared_ptr<_A> _this);
System_Void _A_F_vcall(const std::shared_ptr<_A> _this) {
switch (_this->_typeId)
{
case 3:
_B_F();
return;
case 4:
_A_F();
return;
} //switch
}

#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{
printf("%ls\n", value.get()->Text->Items);
}
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Test_Main()
{
std::shared_ptr<_B> vreg_1;

vreg_1 = std::make_shared<_B >();
vreg_1->_typeId = 3;
_A_F_vcall(vreg_1);
_B_F(vreg_1);
_B_G();
_B_G();
return;
}


System_Void _B_F()
{

System_Console_WriteLine(_str(0));
return;
}


System_Void _A_F()
{

System_Console_WriteLine(_str(1));
return;
}


System_Void _B_G()
{

System_Console_WriteLine(_str(2));
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
_AddJumpAndLength(0, 3);
_AddJumpAndLength(4, 3);
_AddJumpAndLength(8, 3);
} // buildStringTable
const wchar_t _stringTable[12] = {
66, 46, 70, 0 /* "B.F" */, 
65, 46, 70, 0 /* "A.F" */, 
66, 46, 71, 0 /* "B.G" */
}; // _stringTable 

