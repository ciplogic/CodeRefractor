#include "sloth.h"
#include <functional>
struct System_Object;
struct _Complex;
struct System_Console;
struct System_ValueType;
struct System_String;
struct System_Object {
	int _typeId;
};
struct _Complex : public System_Object {
	System_Int32 real;
	System_Int32 imaginary;


};
struct System_Console : public System_Object {
};
struct System_ValueType : public System_Object {
};
struct System_String : public System_Object {
	std::shared_ptr< Array < System_Char > > Text;
};

System_Void _Complex_Main();

System_Void _Complex_ctor(const std::shared_ptr<_Complex>& _this, System_Int32 real, System_Int32 imaginary);

std::shared_ptr<_Complex> _Complex_op_Addition(_Complex * c1, _Complex * c2);

System_Void _Complex_Print(const std::shared_ptr<_Complex>& _this);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{
	printf("%ls\n", value.get()->Text->Items);
}
System_Void System_Console_Write(System_Int32 value)
{
	printf("%d", value);
}
System_Void System_Console_Write(std::shared_ptr<System_String> value)
{
	printf("%ls", value.get()->Text->Items);
}
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Complex_Main()

{
	_Complex * local_0;
	_Complex * local_1;
	_Complex * local_2;
	std::shared_ptr<_Complex> vreg_1;
	std::shared_ptr<_Complex> vreg_3;

	vreg_1 = std::make_shared<_Complex >();
	vreg_1->_typeId = 1;
	_Complex_ctor(vreg_1, 2, 3);
	local_0 = (vreg_1).get();
	vreg_3 = std::make_shared<_Complex >();
	vreg_3->_typeId = 1;
	_Complex_ctor(vreg_3, 3, 4);
	local_1 = (vreg_3).get();
	local_2 = _Complex_op_Addition(local_0, local_1);
	System_Console_WriteLine(_str(1));
	_Complex_Print(local_0);
	System_Console_WriteLine(_str(2));
	_Complex_Print(local_1);
	System_Console_WriteLine(_str(3));
	_Complex_Print(local_2);
	return;
}


System_Void _Complex_ctor(const std::shared_ptr<_Complex>& _this, System_Int32 real, System_Int32 imaginary)

{

	_this->real = real;
	_this->imaginary = imaginary;
	return;
}


std::shared_ptr<_Complex> _Complex_op_Addition(_Complex * c1, _Complex * c2)

{
	System_Int32 vreg_1;
	System_Int32 vreg_2;
	System_Int32 vreg_3;
	System_Int32 vreg_4;
	System_Int32 vreg_5;
	System_Int32 vreg_6;
	std::shared_ptr<_Complex> vreg_7;
	std::shared_ptr<_Complex> vreg_8;

	vreg_1 = c1->real;
	vreg_2 = c2->real;
	vreg_3 = vreg_1 + vreg_2;
	vreg_4 = c1->imaginary;
	vreg_5 = c2->imaginary;
	vreg_6 = vreg_4 + vreg_5;
	vreg_7 = std::make_shared<_Complex >();
	vreg_7->_typeId = 1;
	_Complex_ctor(vreg_7, vreg_3, vreg_6);
	vreg_8 = vreg_7;
	return vreg_8;
}


System_Void _Complex_Print(const std::shared_ptr<_Complex>& _this)

{
	System_Int32 vreg_1;
	System_Int32 vreg_2;

	vreg_1 = _this->real;
	System_Console_Write(vreg_1);
	System_Console_WriteLine(_str(4));
	vreg_2 = _this->imaginary;
	System_Console_Write(vreg_2);
	System_Console_Write(_str(5));
	return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
	auto argsAsList = System_getArgumentsAsList(argc, argv);
	initializeRuntime();
	_Complex_Main();
	return 0;
}
System_Void mapLibs() {
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
	_AddJumpAndLength(0, 15);
	_AddJumpAndLength(16, 23);
	_AddJumpAndLength(40, 23);
	_AddJumpAndLength(64, 28);
	_AddJumpAndLength(93, 4);
	_AddJumpAndLength(98, 1);
} // buildStringTable
const wchar_t _stringTable[100] = {
	80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */,
	70, 105, 114, 115, 116, 32, 99, 111, 109, 112, 108, 101, 120, 32, 110, 117, 109, 98, 101, 114, 58, 32, 32, 0 /* "First complex number:  " */,
	83, 101, 99, 111, 110, 100, 32, 99, 111, 109, 112, 108, 101, 120, 32, 110, 117, 109, 98, 101, 114, 58, 32, 0 /* "Second complex number: " */,
	84, 104, 101, 32, 115, 117, 109, 32, 111, 102, 32, 116, 104, 101, 32, 116, 119, 111, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "The sum of the two numbers: " */,
	32, 43, 32, 105, 0 /* " + i" */,
	105, 0 /* "i" */
}; // _stringTable 

