#include "sloth.h"
#include <functional>
struct System_Object; 
struct System_ValueType; 
struct System_Delegate; 
struct System_MulticastDelegate; 
struct System_Action; 
struct System_String; 
struct ___c__DisplayClass1; 
struct _NBody; 
struct System_Console; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};
struct System_Delegate : public System_Object {
};
struct System_MulticastDelegate : public System_Delegate {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct ___c__DisplayClass1 : public System_Object {
 System_Int32 len;
};
struct _NBody : public System_Object {
};
struct System_Console : public System_Object {
};
struct System_Action : System_Object {
	std::vector< std::function<void()> > _functions;
void Register(std::function<void() > fn) {
_functions.push_back(fn);
}
void Invoke() {
for(auto it : _functions)
	(it)();
}
}; //end of class delegate
void System_Action_ctor(const std::shared_ptr<System_Action>& _delegate, void*, std::function<void()> fn){
  _delegate->Register(fn);
}
void System_Action_Invoke(const std::shared_ptr<System_Action>& _delegate){
  _delegate->Invoke();
}

System_Void _NBody_Main();

System_Void ___c__DisplayClass1_ctor();

System_Void System_Action_ctor();

System_Void System_Action_Invoke();

System_Void ___c__DisplayClass1__Main_b__0(___c__DisplayClass1 * _this);

System_Int32 _NBody_AddPrimes(System_Int32 len);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

typedef void (*System_Action_InvokeVirtPtr)(const std::shared_ptr<System_Action> &_this);
void System_Action_Invoke_vcall(const std::shared_ptr<System_Action> &_this);
void System_Action_Invoke_vcall(const std::shared_ptr<System_Action> &_this){
switch(_this->_typeId)
{
case 4:
System_Action_Invoke(_this);
return;
}
}
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
void (*vreg_3)(___c__DisplayClass1);

___c__DisplayClass1  vreg_1;
vreg_1._typeId = 0;
System_Console_WriteLine(_str(0));
vreg_1->len = 1000000;
vreg_3=&(___c__DisplayClass1__Main_b__0);
return;
}


System_Void ___c__DisplayClass1_ctor()

{

return;
}


System_Void System_Action_ctor()

{

}


System_Void System_Action_Invoke()

{

}


System_Void ___c__DisplayClass1__Main_b__0(___c__DisplayClass1 * _this)

{
System_Int32 local_0;
System_Int32 vreg_1;
System_Int32 vreg_2;

vreg_1 = _this->len;
vreg_2 = _NBody_AddPrimes(vreg_1);
System_Console_Write(vreg_2);
System_Console_WriteLine(_str(1));
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
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
_NBody_Main();
return 0;
}
void mapLibs() {
}

void RuntimeHelpersBuildConstantTable() {
}

void buildStringTable() {
_AddJumpAndLength(0, 15);
_AddJumpAndLength(16, 17);
} // buildStringTable
const wchar_t _stringTable[34] = {
80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */, 
83, 105, 109, 112, 108, 101, 114, 32, 69, 120, 97, 109, 112, 108, 101, 58, 32, 0 /* "Simpler Example: " */
}; // _stringTable 

