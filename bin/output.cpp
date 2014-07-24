#include "sloth.h"
#include <functional>
struct System_Object; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _Test; 
struct _A; 
struct _B; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct System_Object {
int _typeId;
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct _Test : public System_Object {
};
struct _A : public System_Object {
};
struct _B : public _A {
};
struct System_Console : public System_Object {
};

System_Void _Test_Main();

System_Void _B_ctor();

System_Void _B_F();

System_Void _A_F();

System_Void _B_G();

System_Void _A_G();

System_Void _A_ctor();

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();


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
_B_F();
_B_F();
_B_G();
_B_G();
return;
}


System_Void _B_ctor()
{

return;
}


System_Void _B_F()
{

System_Console_WriteLine(_str(1));
return;
}


System_Void _A_F()
{

System_Console_WriteLine(_str(2));
return;
}


System_Void _B_G()
{

System_Console_WriteLine(_str(3));
return;
}


System_Void _A_G()
{

System_Console_WriteLine(_str(4));
return;
}


System_Void _A_ctor()
{

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

std::map<int, std::vector<int> > GlobalMappingType;
System_Void buildTypesTable() {
}

bool IsInstanceOf(int typeSource, int typeImplementation) {
    auto typeVector = GlobalMappingType[typeSource];
	auto begin = typeVector.begin();
	auto end = typeVector.end();
	return std::find(begin, end, typeImplementation)!= end;

}


System_Void buildStringTable() {
_AddJumpAndLength(0, 15);
_AddJumpAndLength(16, 3);
_AddJumpAndLength(20, 3);
_AddJumpAndLength(24, 3);
_AddJumpAndLength(28, 3);
} // buildStringTable
const wchar_t _stringTable[32] = {
80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */, 
66, 46, 70, 0 /* "B.F" */, 
65, 46, 70, 0 /* "A.F" */, 
66, 46, 71, 0 /* "B.G" */, 
65, 46, 71, 0 /* "A.G" */
}; // _stringTable 

