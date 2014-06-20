#include "sloth.h"
#include <functional>
struct System_Object; 
struct _NBody; 
struct System_Console; 
struct System_ValueType; 
struct System_String; 
struct System_Object {
int _typeId;
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct System_ValueType : public System_Object {
};
struct System_Console : public System_Object {
};
struct _NBody : public System_Object {
};

System_Void _NBody_Main();

System_Int32 _NBody_AddPrimes(System_Int32 len);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
System_Void System_Console_Write(System_Int32 value)
{ printf("%d", value); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{
System_Int32 local_0;
System_Int32 local_1;
System_Int32 vreg_1;

System_Console_WriteLine(_str(0));
local_0 = 1000000;
vreg_1 = _NBody_AddPrimes(local_0);
local_1 = vreg_1;
System_Console_Write(local_1);
return;
}


System_Int32 _NBody_AddPrimes(System_Int32 len)

{
System_Int32 local_0;
System_Int32 local_1;
System_Boolean local_2;
System_Int32 local_3;
System_Int32 vreg_1;
System_Int32 vreg_2;
System_Int32 vreg_3;
System_Int32 vreg_4;
System_Int32 vreg_5;
System_Int32 vreg_6;

local_0 = 0;
local_1 = 2;
goto label_2F;
label_6:
vreg_1 = local_1%2;
if(!(vreg_1)) goto label_2B;
local_2 = 1;
local_3 = 2;
goto label_1E;
label_11:
vreg_2 = local_1%local_3;
if(vreg_2) goto label_1A;
local_2 = 0;
goto label_24;
label_1A:
vreg_3 = local_3+1;
local_3 = vreg_3;
label_1E:
vreg_4 = local_3*local_3;
if(vreg_4<=local_1) goto label_11;
label_24:
if(!(local_2)) goto label_2B;
vreg_5 = local_0+1;
local_0 = vreg_5;
label_2B:
vreg_6 = local_1+1;
local_1 = vreg_6;
label_2F:
if(local_1<len) goto label_6;
return local_0;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System_getArgumentsAsList(argc, argv);
initializeRuntime();
_NBody_Main();
return 0;
}
System_Void mapLibs() {
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
_AddJumpAndLength(0, 15);
} // buildStringTable
const wchar_t _stringTable[16] = {
80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */
}; // _stringTable 

