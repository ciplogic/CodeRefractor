#include "sloth.h"
#include <functional>
struct System_Object; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _Test; 
struct _Something; 
struct _Counter; 
struct System_Object {
int _typeId;
};
struct System_String : public System_Object {
System_String() {_typeId = 6; }
 std::shared_ptr< Array < System_Char > > Text;
};
struct _Test : public System_Object {
};
struct _Counter {
	System_Int32 i;
};
struct _Something {
 _Counter c;
};

System_Void _Test_Main();

#include "runtime_base.hpp"
// --- Begin definition of virtual implementingMethod tables ---
System_Void setupTypeTable();


///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Test_Main()
{
_Something local_0;
_Something*  vreg_1;
_Counter*  vreg_2;
_Something*  vreg_3;
_Counter*  vreg_4;
System_Int32 vreg_6;
System_Int32 vreg_7;

vreg_1 = &local_0;
vreg_2 = &(vreg_1->c);
vreg_2->i = 0;
vreg_3 = &local_0;
vreg_4 = &(vreg_3->c);
vreg_6 = vreg_4->i;
vreg_7 = vreg_6+1;
vreg_4->i = vreg_7;
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

