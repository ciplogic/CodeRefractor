#include "sloth.h"
struct System_ValueType; 
struct System_Array; 
struct System_String; 
struct System_Object; 
struct System_Text_StringBuilder; 
struct SimpleAdditions_AuxList_System_Int32; 
struct SimpleAdditions_Program; 
struct System_Console; 
struct System_ValueType {
};
struct System_Array {
};
struct System_String {
 std::shared_ptr< Array < System_Char > > Text;
};
struct System_Object {
};
struct System_Text_StringBuilder {
 std::shared_ptr< Array < System_Char > > _data;
 System_Int32 _writtenLength;
};
struct SimpleAdditions_AuxList_System_Int32 {
 std::shared_ptr< Array < System_Int32 > > Data;
 System_Int32 AutoNamed_0;
};
struct SimpleAdditions_Program {
};
struct System_Console {
};

System_Void SimpleAdditions_Program_Main();

System_Void SimpleAdditions_AuxList_System_Int32_ctor(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  capacity);

System_Void SimpleAdditions_AuxList_System_Int32_Add(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  item);

System_Int32 SimpleAdditions_AuxList_System_Int32_Get(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  index);

System_Int32 SimpleAdditions_AuxList_System_Int32_get_Count(SimpleAdditions_AuxList_System_Int32 * _this);

System_Int32 SimpleAdditions_AuxList_System_Int32_get_Capacity(SimpleAdditions_AuxList_System_Int32 * _this);

System_Void SimpleAdditions_AuxList_System_Int32_set_Count(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  value);

#include "runtime_base.hpp"
#include "stdio.h"
System_Void System_Console_WriteLine(System_Int32 value)
{ printf("%d\n", value); }
///---Begin closure code --- 
System_Void SimpleAdditions_Program_Main()
{
System_Int32 vreg_8;

SimpleAdditions_AuxList_System_Int32  vreg_2;
SimpleAdditions_AuxList_System_Int32_ctor(&vreg_2, 0);
SimpleAdditions_AuxList_System_Int32_Add(&vreg_2, 1);
vreg_8 = SimpleAdditions_AuxList_System_Int32_Get(&vreg_2, 0);
System_Console_WriteLine(vreg_8);
return;
}


System_Void SimpleAdditions_AuxList_System_Int32_ctor(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  capacity)
{
std::shared_ptr< Array < System_Int32 > > vreg_4;

vreg_4 = std::make_shared< Array <System_Int32> >(capacity); 
_this->Data = vreg_4;
return;
}


System_Void SimpleAdditions_AuxList_System_Int32_Add(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  item)
{
System_Boolean local_0;
System_Int32 vreg_2;
System_Int32 vreg_4;
System_Boolean vreg_6;
System_Int32 vreg_8;
System_Int32 vreg_10;
System_Boolean vreg_13;
std::shared_ptr< Array < System_Int32 > >*  vreg_15;
Array < System_Int32 > * vreg_18;
System_Int32 vreg_20;
System_Int32 vreg_24;
System_Int32 vreg_26;

vreg_2 = SimpleAdditions_AuxList_System_Int32_get_Count(_this);
vreg_4 = SimpleAdditions_AuxList_System_Int32_get_Capacity(_this);
vreg_6 = (vreg_2 > vreg_4)?1:0;
if(vreg_6) goto label_52;
vreg_8 = SimpleAdditions_AuxList_System_Int32_get_Capacity(_this);
vreg_10 = (vreg_8 == 0)?1:0;
vreg_13 = (vreg_10 == 0)?1:0;
if(vreg_13) goto label_52;
vreg_15 = &_this->Data;
System_Array_Resize(vreg_15, 10);
label_52:
vreg_18 = _this->Data.get();
vreg_20 = SimpleAdditions_AuxList_System_Int32_get_Count(_this);
(*vreg_18)[vreg_20] = item; 
vreg_24 = SimpleAdditions_AuxList_System_Int32_get_Count(_this);
vreg_26 = vreg_24+1;
SimpleAdditions_AuxList_System_Int32_set_Count(_this, vreg_26);
return;
}


System_Int32 SimpleAdditions_AuxList_System_Int32_Get(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  index)
{
System_Int32 local_0;
Array < System_Int32 > * vreg_2;
System_Int32 vreg_4;

vreg_2 = _this->Data.get();
vreg_4 = (*vreg_2)[index];
return vreg_4;
}


System_Int32 SimpleAdditions_AuxList_System_Int32_get_Count(SimpleAdditions_AuxList_System_Int32 * _this)
{
System_Int32 local_0;
System_Int32 vreg_2;

vreg_2 = _this->AutoNamed_0;
return vreg_2;
}


System_Int32 SimpleAdditions_AuxList_System_Int32_get_Capacity(SimpleAdditions_AuxList_System_Int32 * _this)
{
System_Int32 local_0;
Array < System_Int32 > * vreg_2;
System_Int32 vreg_3;

vreg_2 = _this->Data.get();
vreg_3 = vreg_2->Length;
local_0 = (int)vreg_3;
return local_0;
}


System_Void SimpleAdditions_AuxList_System_Int32_set_Count(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  value)
{

_this->AutoNamed_0 = value;
return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
SimpleAdditions_Program_Main();
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

