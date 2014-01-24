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
template<class T1> 
struct SimpleAdditions_AuxList
{
 std::shared_ptr< Array < T1 > > Data;
 System_Int32 AutoNamed_0;
};

System_Void SimpleAdditions_Program_Main();

System_Void SimpleAdditions_AuxList_System_Int32_ctor(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  capacity);

System_Void SimpleAdditions_AuxList_System_Int32_Add(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  item);

System_Int32 SimpleAdditions_AuxList_System_Int32_Get(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  index);

System_Void SimpleAdditions_AuxList_System_Int32_set_Count(SimpleAdditions_AuxList_System_Int32 * _this, System_Int32  value);

System_Int32 SimpleAdditions_AuxList_System_Int32_get_Count(SimpleAdditions_AuxList_System_Int32 * _this);

System_Int32 SimpleAdditions_AuxList_System_Int32_get_Capacity(SimpleAdditions_AuxList_System_Int32 * _this);



template<class T1> 
System_Void SimpleAdditions_AuxList_ctor(SimpleAdditions_AuxList<T1> * _this, System_Int32  capacity);

template<class T1> 
System_Void SimpleAdditions_AuxList_set_Count(SimpleAdditions_AuxList<T1> * _this, System_Int32  value);

template<class T1> 
System_Void SimpleAdditions_AuxList_ctor(SimpleAdditions_AuxList<T1> * _this, System_Int32  capacity)
{
std::shared_ptr< Array < T1 > > vreg_6;

SimpleAdditions_AuxList_set_Count<T1>(_this, 0);
vreg_6 = std::make_shared< Array <System_Int32> >(capacity); 
_this->Data = vreg_6;
return;
}

#include "runtime_base.hpp"
#include "stdio.h"
System_Void System_Console_WriteLine(System_Int32 value)
{ printf("%d\n", value); }
///---Begin closure code --- 
System_Void SimpleAdditions_Program_Main()
{
System_Int32 vreg_8;

SimpleAdditions_AuxList<System_Int32> vreg_2;
SimpleAdditions_AuxList_ctor<System_Int32>(&vreg_2, 0);
return;
}



template<class T1> 
System_Void SimpleAdditions_AuxList_System_Int32_ctor(SimpleAdditions_AuxList<T1> * _this, System_Int32  capacity)
{
std::shared_ptr< Array < T1 > > vreg_6;

SimpleAdditions_AuxList_set_Count<T1>(_this, 0);
vreg_6 = std::make_shared< Array <System_Int32> >(capacity); 
_this->Data = vreg_6;
return;
}

template<class T1> 
System_Void SimpleAdditions_AuxList_set_Count(SimpleAdditions_AuxList<T1> * _this, System_Int32  value)
{

_this->AutoNamed_0 = value;
return;
}

template<class T1> 
System_Int32 SimpleAdditions_AuxList_get_Count(SimpleAdditions_AuxList<T1> * _this)
{
System_Int32 local_0;
System_Int32 vreg_2;

vreg_2 = _this->AutoNamed_0;
return vreg_2;
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

