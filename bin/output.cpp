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

System_Void _A_Test(const std::shared_ptr<_A>& _this);

std::shared_ptr<_B> _A_NotAnA(const std::shared_ptr<_A>& _this, System_Int32 count, std::shared_ptr<System_String> message);

System_Void _B_Test(const std::shared_ptr<_B>& _this);

System_Void _C_Test(const std::shared_ptr<_C>& _this);

std::shared_ptr<_B> _B_NotAnA(const std::shared_ptr<_B>& _this, System_Int32 count, std::shared_ptr<System_String> message);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

typedef System_Void(*_A_TestVirtPtr)(const std::shared_ptr<_A> _this);
System_Void _A_Test_vcall(const std::shared_ptr<_A> _this);
typedef std::shared_ptr<_B>(*_A_NotAnAVirtPtr)(const std::shared_ptr<_A> _this, System_Int32 param0, std::shared_ptr<System_String> param1);
std::shared_ptr<_B> _A_NotAnA_vcall(const std::shared_ptr<_A> _this, System_Int32 param0, std::shared_ptr<System_String> param1);
System_Void _A_Test_vcall(const std::shared_ptr<_A> _this){
	switch (_this->_typeId)
	{
	case 2:
		_A_Test(std::static_pointer_cast<_A>(_this));
		return;
	case 3:
		_B_Test(std::static_pointer_cast<_B>(_this));
		return;
	case 4:
		_C_Test(std::static_pointer_cast<_C>(_this));
		return;
	case 5:
		_C_Test(std::static_pointer_cast<_C>(_this));
		return;
	}
}
std::shared_ptr<_B> _A_NotAnA_vcall(const std::shared_ptr<_A> _this, System_Int32 param0, std::shared_ptr<System_String> param1){
	switch (_this->_typeId)
	{
	case 2:
		return _A_NotAnA(std::static_pointer_cast<_A>(_this), param0, param1);
	case 3:
		return _B_NotAnA(std::static_pointer_cast<_B>(_this), param0, param1);
	case 4:
		return _B_NotAnA(std::static_pointer_cast<_B>(_this), param0, param1);
	case 5:
		return _B_NotAnA(std::static_pointer_cast<_B>(_this), param0, param1);
	}
}
// --- End of definition of virtual method tables ---

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{
	printf("%ls\n", value.get()->Text->Items);
}
System_Void System_Console_WriteLine(System_Int32 value)
{
	printf("%d\n", value);
}
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Program_Main()

{
	std::shared_ptr<_A> local_0;
	std::shared_ptr<_B> local_1;
	std::shared_ptr<_A> local_2;
	std::shared_ptr<_B> local_3;
	std::shared_ptr<_A> local_4;
	std::shared_ptr<_A> local_5;
	std::shared_ptr<_A> vreg_1;
	std::shared_ptr<_A> vreg_2;
	std::shared_ptr<_B> vreg_3;
	std::shared_ptr<_B> vreg_4;
	std::shared_ptr<_B> vreg_5;
	std::shared_ptr<_B> vreg_6;
	std::shared_ptr<_C> vreg_7;
	std::shared_ptr<_C> vreg_8;
	std::shared_ptr<_D> vreg_9;
	std::shared_ptr<_D> vreg_10;

	vreg_1 = std::make_shared<_A >();
	vreg_1->_typeId = 2;
	_A_ctor(vreg_1);
	vreg_2 = vreg_1;
	local_0 = vreg_2;
	_A_Test_vcall(local_0);
	vreg_3 = _A_NotAnA_vcall(local_0, 0, _str(1));
	local_1 = vreg_3;
	_A_Test_vcall(local_1);
	vreg_4 = std::make_shared<_B >();
	vreg_4->_typeId = 3;
	_B_ctor(vreg_4);
	vreg_5 = vreg_4;
	local_2 = vreg_5;
	_A_Test_vcall(local_2);
	vreg_6 = _A_NotAnA_vcall(local_2, 0, _str(2));
	local_3 = vreg_6;
	_A_Test_vcall(local_3);
	vreg_7 = std::make_shared<_C >();
	vreg_7->_typeId = 4;
	_C_ctor(vreg_7);
	vreg_8 = vreg_7;
	local_4 = vreg_8;
	_A_Test_vcall(local_4);
	vreg_9 = std::make_shared<_D >();
	vreg_9->_typeId = 5;
	_D_ctor(vreg_9);
	vreg_10 = vreg_9;
	local_5 = vreg_10;
	_A_Test_vcall(local_5);
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


System_Void _A_Test(const std::shared_ptr<_A>& _this)

{

	System_Console_WriteLine(_str(3));
	return;
}


std::shared_ptr<_B> _A_NotAnA(const std::shared_ptr<_A>& _this, System_Int32 count, std::shared_ptr<System_String> message)

{
	std::shared_ptr<_B> vreg_1;
	std::shared_ptr<_B> vreg_2;

	System_Console_WriteLine(count);
	System_Console_WriteLine(message);
	System_Console_WriteLine(_str(3));
	vreg_1 = std::make_shared<_B >();
	vreg_1->_typeId = 3;
	_B_ctor(vreg_1);
	vreg_2 = vreg_1;
	return vreg_2;
}


System_Void _B_Test(const std::shared_ptr<_B>& _this)

{

	System_Console_WriteLine(_str(4));
	return;
}


System_Void _C_Test(const std::shared_ptr<_C>& _this)

{

	System_Console_WriteLine(_str(5));
	return;
}


std::shared_ptr<_B> _B_NotAnA(const std::shared_ptr<_B>& _this, System_Int32 count, std::shared_ptr<System_String> message)

{

	System_Console_WriteLine(count);
	System_Console_WriteLine(message);
	System_Console_WriteLine(_str(3));
	return _this;
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
	_AddJumpAndLength(16, 5);
	_AddJumpAndLength(22, 5);
	_AddJumpAndLength(28, 6);
	_AddJumpAndLength(35, 6);
	_AddJumpAndLength(42, 6);
} // buildStringTable
const wchar_t _stringTable[49] = {
	80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */,
	116, 101, 115, 116, 49, 0 /* "test1" */,
	116, 101, 115, 116, 50, 0 /* "test2" */,
	65, 46, 84, 101, 115, 116, 0 /* "A.Test" */,
	66, 46, 84, 101, 115, 116, 0 /* "B.Test" */,
	67, 46, 84, 101, 115, 116, 0 /* "C.Test" */
}; // _stringTable 

