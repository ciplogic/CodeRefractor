#include "sloth.h"
#include <functional>
struct System_Object; 
struct _MyArray; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _NBody; 
struct System_Object {
int _typeId;
};
struct _MyList_1System_Int32 : public System_Object {
 std::shared_ptr< Array < System_Int32 > > _items;
 System_Int32 AutoNamed_0;
};
struct _MyArray : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct _NBody : public System_Object {
};

System_Void _NBody_Main();

System_Void _MyList_1System_Int32_ctor(const std::shared_ptr<_MyList_1System_Int32>& _this);

System_Void _MyList_1System_Int32_Add(const std::shared_ptr<_MyList_1System_Int32>& _this, System_Int32 value);

System_Void _MyList_1System_Int32_set_Capacity(const std::shared_ptr<_MyList_1System_Int32>& _this, System_Int32 value);

System_Int32 _MyList_1System_Int32_get_Count(const std::shared_ptr<_MyList_1System_Int32>& _this);

System_Int32 _MyList_1System_Int32_get_Capacity(const std::shared_ptr<_MyList_1System_Int32>& _this);

System_Void _MyList_1System_Int32_set_Count(const std::shared_ptr<_MyList_1System_Int32>& _this, System_Int32 value);

System_Void _MyArray_Resize(std::shared_ptr< Array < System_Int32 > >*  array, System_Int32 newSize);

System_Void _MyArray_Copy(Array < System_Int32 > * dest, Array < System_Int32 > * src, System_Int32  size);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()
{
std::shared_ptr<_MyList_1System_Int32> vreg_1;

vreg_1 = std::make_shared<_MyList_1System_Int32 >();
vreg_1->_typeId = 3;
_MyList_1System_Int32_ctor(vreg_1);
_MyList_1System_Int32_Add(vreg_1, 2);
return;
}


System_Void _MyList_1System_Int32_ctor(const std::shared_ptr<_MyList_1System_Int32>& _this)
{

_this->_items = nullptr;
_MyList_1System_Int32_set_Capacity(_this, 8);
return;
}


System_Void _MyList_1System_Int32_Add(const std::shared_ptr<_MyList_1System_Int32>& _this, System_Int32 value)
{
System_Int32 local_0;
System_Boolean local_1;
System_Int32 vreg_1;
System_Int32 vreg_2;
System_Int32 vreg_3;
System_Int32 vreg_6;
System_Int32 vreg_7;
std::shared_ptr< Array < System_Int32 > > vreg_8;

local_0 = vreg_1;
vreg_2 = _MyList_1System_Int32_get_Capacity(_this);
vreg_3 = (vreg_1 == vreg_2)?1:0;
local_1 = (vreg_3 == 0)?1:0;
if(local_1) goto label_29;
vreg_6 = _MyList_1System_Int32_get_Capacity(_this);
vreg_7 = vreg_6*2;
_MyList_1System_Int32_set_Capacity(_this, vreg_7);
label_29:
vreg_8 = _this->_items;
(*vreg_8)[local_0] = value; 
return;
}


System_Void _MyList_1System_Int32_set_Capacity(const std::shared_ptr<_MyList_1System_Int32>& _this, System_Int32 value)
{
System_Int32 local_0;
System_Boolean local_1;
std::shared_ptr< Array < System_Int32 > > vreg_1;
std::shared_ptr< Array < System_Int32 > > vreg_2;
std::shared_ptr< Array < System_Int32 > > vreg_4;
std::shared_ptr< Array < System_Int32 > >*  vreg_5;

local_0 = value;
vreg_1 = _this->_items;
vreg_2 = (vreg_1 == nullptr)?1:0;
local_1 = (vreg_2 == 0)?1:0;
if(local_1) goto label_23;
vreg_4 = std::make_shared< Array <System_Int32> >(local_0); 
_this->_items = vreg_4;
goto label_32;
label_23:
vreg_5 = &_this->_items;
_MyArray_Resize(vreg_5, local_0);
label_32:
return;
}


System_Int32 _MyList_1System_Int32_get_Count(const std::shared_ptr<_MyList_1System_Int32>& _this)
{
System_Int32 local_0;

local_0 = _this->AutoNamed_0;
return local_0;
}


System_Int32 _MyList_1System_Int32_get_Capacity(const std::shared_ptr<_MyList_1System_Int32>& _this)
{
System_Int32 local_0;
std::shared_ptr< Array < System_Int32 > > vreg_1;
System_Int32 vreg_2;

vreg_1 = _this->_items;
vreg_2 = vreg_1->Length;
local_0 = (int)vreg_2;
return local_0;
}


System_Void _MyList_1System_Int32_set_Count(const std::shared_ptr<_MyList_1System_Int32>& _this, System_Int32 value)
{

_this->AutoNamed_0 = value;
return;
}


System_Void _MyArray_Resize(std::shared_ptr< Array < System_Int32 > >*  array, System_Int32 newSize)
{
std::shared_ptr< Array < System_Int32 > > local_0;
System_Boolean local_1;
std::shared_ptr< Array < System_Int32 > > vreg_1;
std::shared_ptr< Array < System_Int32 > > vreg_2;
std::shared_ptr< Array < System_Int32 > > vreg_5;
System_Int32 vreg_6;
System_Int32 vreg_7;
System_Int32 vreg_8;
std::shared_ptr< Array < System_Int32 > > vreg_10;
std::shared_ptr< Array < System_Int32 > > vreg_11;
std::shared_ptr< Array < System_Int32 > > vreg_12;
System_Int32 vreg_13;
System_Int32 vreg_14;

vreg_1 = *array;
vreg_2 = (vreg_1 == nullptr)?1:0;
local_1 = (vreg_2 == 0)?1:0;
if(local_1) goto label_18;
array = std::make_shared< Array <System_Int32> >(newSize); 
goto label_3F;
label_18:
vreg_5 = *array;
vreg_6 = vreg_5->Length;
vreg_7 = (int)vreg_6;
vreg_8 = (vreg_7 == newSize)?1:0;
local_1 = (vreg_8 == 0)?1:0;
if(local_1) goto label_28;
goto label_3F;
label_28:
vreg_10 = std::make_shared< Array <System_Int32> >(newSize); 
vreg_11 = *array;
vreg_12 = *array;
vreg_13 = vreg_12->Length;
vreg_14 = (int)vreg_13;
_MyArray_Copy(vreg_10, vreg_11, vreg_14);
*array = vreg_10;
label_3F:
return;
}


System_Void _MyArray_Copy(Array < System_Int32 > * dest, Array < System_Int32 > * src, System_Int32  size)
{
System_Int32 local_0;
System_Int32 local_1;
System_Boolean local_2;
System_Int32 vreg_1;

local_0 = 0;
goto label_1B;
label_5:
vreg_1 = (*src)[local_0];
(*dest)[local_0] = vreg_1; 
local_0 = local_0+1;
label_1B:
local_2 = (local_0 < size)?1:0;
if(local_2) goto label_5;
return;
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
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

