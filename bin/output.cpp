#include "sloth.h"
#include <functional>
struct System_Object; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct _TestShapes; 
struct CodeRefactor_OpenRuntime_CrString; 
struct System_Object {
int _typeId;
};
struct System_Console : public System_Object {
};
struct _TestShapes : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};

System_Void _TestShapes_Main();

std::shared_ptr<System_String> CodeRefactor_OpenRuntime_StringImpl_Concat(std::shared_ptr<CodeRefactor_OpenRuntime_CrString> s1, std::shared_ptr<CodeRefactor_OpenRuntime_CrString> s2);

System_Void System_String_ctor(const std::shared_ptr<CodeRefactor_OpenRuntime_CrString>& _this, System_Char value);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void System_Console_Write(std::shared_ptr<System_String> value)
{ printf("%ls", value.get()->Text->Items); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _TestShapes_Main()
{
std::shared_ptr<System_String> vreg_1;

vreg_1 = System_String_Concat(_str(0), _str(1));
System_Console_Write(vreg_1);
return;
}


std::shared_ptr<System_String> CodeRefactor_OpenRuntime_StringImpl_Concat(std::shared_ptr<CodeRefactor_OpenRuntime_CrString> s1, std::shared_ptr<CodeRefactor_OpenRuntime_CrString> s2)
{
Array < System_Char >  * local_0;
Array < System_Char >  * local_1;
std::shared_ptr< Array < System_Char > > local_2;
System_Int32 local_3;
System_Int32 local_4;
std::shared_ptr<System_String> local_5;
System_Boolean local_6;
Array < System_Char >  * vreg_1;
Array < System_Char >  * vreg_2;
System_Int32 vreg_5;
System_Int32 vreg_6;
System_Int32 vreg_7;
std::shared_ptr< Array < System_Char > > vreg_8;
System_Char vreg_9;
System_Int32 vreg_12;
System_Int32 vreg_14;
System_Int32 vreg_15;
System_Int32 vreg_16;
System_Char vreg_17;
System_Int32 vreg_19;
System_Int32 vreg_20;
std::shared_ptr<System_String> vreg_22;
System_Int32 vreg_23;
System_Int32 vreg_24;

vreg_1 = System_String_ToCharArray(s1);
local_0 = vreg_1;
vreg_2 = System_String_ToCharArray(s2);
local_1 = vreg_2;
vreg_23 = vreg_1->Length;
vreg_24 = (int)vreg_23;
vreg_5 = vreg_2->Length;
vreg_6 = (int)vreg_5;
vreg_7 = vreg_24+vreg_6;
vreg_8 = std::make_shared< Array <System_Char> >(vreg_7); 
local_2 = vreg_8;
local_3 = 0;
vreg_12 = vreg_24;
goto label_2C;
label_20:
vreg_9 = (*local_0)[local_3];
(*local_2)[local_3] = vreg_9; 
local_3 = local_3+1;
label_2C:
local_6 = (local_3 < vreg_12)?1:0;
if(local_6) goto label_20;
vreg_14 = local_0->Length;
vreg_15 = (int)vreg_14;
local_4 = vreg_15;
local_3 = 0;
vreg_19 = local_1->Length;
vreg_20 = (int)vreg_19;
goto label_50;
label_41:
vreg_16 = local_3+local_4;
vreg_17 = (*local_1)[local_3];
(*local_2)[vreg_16] = vreg_17; 
local_3 = local_3+1;
label_50:
local_6 = (local_3 < vreg_20)?1:0;
if(local_6) goto label_41;
vreg_22 = std::make_shared<System_String >();
vreg_22->_typeId = 0;
System_String_ctor(vreg_22, local_2);
local_5 = vreg_22;
label_66:
return local_5;
}


System_Void System_String_ctor(const std::shared_ptr<CodeRefactor_OpenRuntime_CrString>& _this, System_Char value)
{
System_Int32 local_0;
System_Int32 local_1;
System_Boolean local_2;
System_Int32 vreg_1;
System_Int32 vreg_2;
std::shared_ptr< Array < System_Char > > vreg_3;
Array < System_Char >  * vreg_4;
System_Char vreg_5;
System_Int32 vreg_7;

vreg_1 = value->Length;
vreg_2 = (int)vreg_1;
local_0 = vreg_2;
vreg_3 = std::make_shared< Array <System_Char> >(vreg_2); 
_this->Text = vreg_3;
local_1 = 0;
vreg_4 = _this->Text.get();
goto label_2B;
label_1C:
vreg_5 = (*value)[local_1];
(*vreg_4)[local_1] = vreg_5; 
local_1 = local_1+1;
label_2B:
vreg_7 = (local_1 > local_0)?1:0;
local_2 = (vreg_7 == 0)?1:0;
if(local_2) goto label_1C;
return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System_getArgumentsAsList(argc, argv);
initializeRuntime();
_TestShapes_Main();
return 0;
}
System_Void mapLibs() {
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
_AddJumpAndLength(0, 5);
_AddJumpAndLength(6, 6);
} // buildStringTable
const wchar_t _stringTable[13] = {
72, 101, 108, 108, 111, 0 /* "Hello" */, 
32, 119, 111, 114, 108, 100, 0 /* " world" */
}; // _stringTable 

