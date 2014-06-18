#include "sloth.h"
#include <functional>
struct System_Object; 
struct System_ValueType; 
struct _NBody; 
struct System_Console; 
struct System_String; 
struct System_Object {
int _typeId;
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct System_Console : public System_Object {
};
struct _NBody : public System_Object {
};
struct System_ValueType : public System_Object {
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

System_Console_WriteLine(_str(0));
System_Console_Write(78497);
return;
}


System_Int32 _NBody_AddPrimes(System_Int32 len)

{
System_Int32 local_0;
System_Int32 local_1;
System_Int32 local_3;
System_Int32 vreg_1;
System_Int32 vreg_2;
System_Int32 vreg_3;
System_Int32 vreg_4;
System_Int32 vreg_5;
System_Int32 vreg_6;
System_Int32 vreg_8;
System_Int32 vreg_9;
System_Int32 vreg_10;
System_Int32 vreg_14;

local_0 = 0;
local_1 = 2;
local_3 = 2;
goto label_5B;
label_7:
vreg_1 = local_1%2;
vreg_2 = (vreg_1 == 0)?1:0;
vreg_3 = (vreg_2 == 0)?1:0;
if(vreg_3) goto label_19;
goto label_57;
label_19:
goto label_39;
label_1F:
vreg_4 = local_1%local_3;
vreg_5 = (vreg_4 == 0)?1:0;
vreg_6 = (vreg_5 == 0)?1:0;
if(vreg_6) goto label_34;
goto label_48;
label_34:
local_3 = local_3+1;
label_39:
vreg_8 = local_3*local_3;
vreg_9 = (vreg_8 > local_1)?1:0;
vreg_10 = (vreg_9 == 0)?1:0;
if(vreg_10) goto label_1F;
label_48:
local_0 = local_0+1;
label_57:
local_1 = local_1+1;
label_5B:
vreg_14 = (local_1 < len)?1:0;
if(vreg_14) goto label_7;
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

