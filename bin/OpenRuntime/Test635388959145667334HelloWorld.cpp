#include "sloth.h"
#include <functional>
struct System_Object; 
struct _Test; 
struct _C; 
struct _A; 
struct _B; 
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
struct _A : public System_Object {
};
struct _Test : public System_Object {
};
struct _C : public _B {
};
struct _B : public _A {
};

System_Void _Test_Main();

System_Void _C_ctor(_C * _this);

System_Void _A_F(_A * _this);

System_Void _B_F(_B * _this);

System_Void _C_F(_C * _this);

System_Void _A_G(_A * _this);

System_Void _C_G(_C * _this);

System_Void _B_G(_B * _this);

System_Void _B_ctor(_B * _this);

System_Void _A_ctor(_A * _this);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

typedef System_Void (*_A_GVirtPtr)(const _A * _this);
System_Void _A_G_vcall(const _A * _this);
System_Void _A_G_vcall(const _A * _this){
switch(_this->_typeId)
{
case 2:
_C_G(_this);
return;
case 3:
_A_G(_this);
return;
case 4:
_B_G(_this);
return;
}
}
// --- End of definition of virtual method tables ---

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Test_Main()

{
std::shared_ptr<_C> local_0;
std::shared_ptr<_A> local_1;
std::shared_ptr<_B> local_2;
std::shared_ptr<_C> vreg_1;
std::shared_ptr<_C> vreg_2;

vreg_1 = std::make_shared<_C >();
vreg_1->_typeId = 2;
_C_ctor(vreg_1.get());
vreg_2 = vreg_1;
local_0 = vreg_2;
local_1 = local_0;
local_2 = local_0;
_A_F_vcall(local_1.get());
_B_F_vcall(local_2.get());
_C_F(local_0.get());
_A_G_vcall(local_1.get());
_B_G_vcall(local_2.get());
_C_G_vcall(local_0.get());
return;
}


System_Void _C_ctor(_C * _this)

{

_B_ctor(_this);
return;
}


System_Void _A_F(_A * _this)

{

System_Console_WriteLine(_str(1));
return;
}


System_Void _B_F(_B * _this)

{

System_Console_WriteLine(_str(2));
return;
}


System_Void _C_F(_C * _this)

{

System_Console_WriteLine(_str(6));
return;
}


System_Void _A_G(_A * _this)

{

System_Console_WriteLine(_str(3));
return;
}


System_Void _C_G(_C * _this)

{

System_Console_WriteLine(_str(5));
return;
}


System_Void _B_G(_B * _this)

{

System_Console_WriteLine(_str(4));
return;
}


System_Void _B_ctor(_B * _this)

{

_A_ctor(_this);
return;
}


System_Void _A_ctor(_A * _this)

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

System_Void buildStringTable() {
_AddJumpAndLength(0, 15);
_AddJumpAndLength(16, 3);
_AddJumpAndLength(20, 3);
_AddJumpAndLength(24, 3);
_AddJumpAndLength(28, 3);
_AddJumpAndLength(32, 3);
_AddJumpAndLength(36, 3);
} // buildStringTable
const wchar_t _stringTable[40] = {
80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */, 
65, 46, 70, 0 /* "A.F" */, 
66, 46, 70, 0 /* "B.F" */, 
65, 46, 71, 0 /* "A.G" */, 
66, 46, 71, 0 /* "B.G" */, 
67, 46, 71, 0 /* "C.G" */, 
67, 46, 70, 0 /* "C.F" */
}; // _stringTable 

