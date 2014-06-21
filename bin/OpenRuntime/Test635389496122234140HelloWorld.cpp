#include "sloth.h"
#include <functional>
struct System_Object; 
struct _Program; 
struct _A; 
struct _B; 
struct _C; 
struct _D; 
struct System_Console; 
struct System_ValueType; 
struct System_String; 
struct System_Object {
int _typeId;
};
struct _Program : public System_Object {
};
struct _A : public System_Object {
};
struct _B : public _A {
};
struct _C : public _B {
};
struct _D : public _C {
};
struct System_Console : public System_Object {
};
struct System_ValueType : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};

System_Void _Program_Main();

System_Void _A_ctor(const std::shared_ptr<_A>& _this);

System_Void _B_ctor(const std::shared_ptr<_B>& _this);

System_Void _C_ctor(const std::shared_ptr<_C>& _this);

System_Void _D_ctor(const std::shared_ptr<_D>& _this);

System_Void _A_Test(_A * _this);

System_Void _B_Test(_B * _this);

System_Void _C_Test(_C * _this);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

typedef System_Void (*_A_TestVirtPtr)(const _A * _this);
System_Void _A_Test_vcall(const _A * _this);
System_Void _A_Test_vcall(const _A * _this){
switch(_this->_typeId)
{
case 2:
_A_Test(_this);
return;
case 3:
_B_Test(_this);
return;
case 4:
_C_Test(_this);
return;
case 5:
_C_Test(_this);
return;
}
}
// --- End of definition of virtual method tables ---

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Program_Main()

{
std::shared_ptr<_A> local_0;
std::shared_ptr<_A> local_1;
std::shared_ptr<_A> local_2;
std::shared_ptr<_A> local_3;
std::shared_ptr<_A> vreg_1;
std::shared_ptr<_A> vreg_2;
std::shared_ptr<_B> vreg_3;
std::shared_ptr<_B> vreg_4;
std::shared_ptr<_C> vreg_5;
std::shared_ptr<_C> vreg_6;
std::shared_ptr<_D> vreg_7;
std::shared_ptr<_D> vreg_8;

vreg_1 = std::make_shared<_A >();
vreg_1->_typeId = 2;
_A_ctor(vreg_1);
vreg_2 = vreg_1;
local_0 = vreg_2;
_A_Test_vcall(local_0.get());
vreg_3 = std::make_shared<_B >();
vreg_3->_typeId = 3;
_B_ctor(vreg_3);
vreg_4 = vreg_3;
local_1 = vreg_4;
_A_Test_vcall(local_1.get());
vreg_5 = std::make_shared<_C >();
vreg_5->_typeId = 4;
_C_ctor(vreg_5);
vreg_6 = vreg_5;
local_2 = vreg_6;
_A_Test_vcall(local_2.get());
vreg_7 = std::make_shared<_D >();
vreg_7->_typeId = 5;
_D_ctor(vreg_7);
vreg_8 = vreg_7;
local_3 = vreg_8;
_A_Test_vcall(local_3.get());
return;
}


System_Void _A_ctor(const std::shared_ptr<_A>& _this)

{

return;
}


System_Void _B_ctor(const std::shared_ptr<_B>& _this)

{

_A_ctor(_this);
return;
}


System_Void _C_ctor(const std::shared_ptr<_C>& _this)

{

_B_ctor(_this);
return;
}


System_Void _D_ctor(const std::shared_ptr<_D>& _this)

{

_C_ctor(_this);
return;
}


System_Void _A_Test(_A * _this)

{

System_Console_WriteLine(_str(1));
return;
}


System_Void _B_Test(_B * _this)

{

System_Console_WriteLine(_str(2));
return;
}


System_Void _C_Test(_C * _this)

{

System_Console_WriteLine(_str(3));
_C_Test(_this);
return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System_getArgumentsAsList(argc, argv);
initializeRuntime();
_Program_Main();
return 0;
}
System_Void mapLibs() {
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
_AddJumpAndLength(0, 15);
_AddJumpAndLength(16, 6);
_AddJumpAndLength(23, 6);
_AddJumpAndLength(30, 6);
} // buildStringTable
const wchar_t _stringTable[37] = {
80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */, 
65, 46, 84, 101, 115, 116, 0 /* "A.Test" */, 
66, 46, 84, 101, 115, 116, 0 /* "B.Test" */, 
67, 46, 84, 101, 115, 116, 0 /* "C.Test" */
}; // _stringTable 

