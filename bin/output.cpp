#include "sloth.h"
#include <functional>
struct System_Object; 
struct _Shape; 
struct _Square; 
struct _Cube; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _TestShapes; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct System_Object {
int _typeId;
};
struct _Shape : public System_Object {
};
struct _Square : public _Shape {
 System_Double side;
};
struct _Cube : public _Shape {
 System_Double side;
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct _TestShapes : public System_Object {
};
struct System_Console : public System_Object {
};

System_Void _TestShapes_Main();

System_Void _Square_ctor(const std::shared_ptr<_Square>& _this, System_Double s);

System_Void _Cube_ctor(const std::shared_ptr<_Cube>& _this, System_Double s);

System_Void _Shape_ctor();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void CodeRefactor_OpenRuntime_CrConsole_Write(std::shared_ptr<System_String> value)
{ printf("%ls", value.get()->Text->Items); }
System_Void CodeRefactor_OpenRuntime_CrConsole_WriteLine(System_Double value)
{ printf("%lf\n", value); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _TestShapes_Main()

{
std::shared_ptr<_Square> vreg_1;
std::shared_ptr<_Cube> vreg_3;
System_Double vreg_5;
System_Double vreg_6;
System_Double vreg_7;
System_Double vreg_8;

CodeRefactor_OpenRuntime_CrConsole_Write(_str(0));
vreg_1 = std::make_shared<_Square >();
vreg_1->_typeId = 4;
vreg_3 = std::make_shared<_Cube >();
vreg_3->_typeId = 6;
CodeRefactor_OpenRuntime_CrConsole_Write(_str(1));
vreg_5 = _Shape_get_Area_vcall(vreg_1);
CodeRefactor_OpenRuntime_CrConsole_WriteLine(vreg_5);
CodeRefactor_OpenRuntime_CrConsole_Write(_str(2));
vreg_6 = _Shape_get_Area_vcall(vreg_3);
CodeRefactor_OpenRuntime_CrConsole_WriteLine(vreg_6);
CodeRefactor_OpenRuntime_CrConsole_Write(_str(3));
_Shape_set_Area_vcall(vreg_1, 50);
_Shape_set_Area_vcall(vreg_3, 50);
CodeRefactor_OpenRuntime_CrConsole_Write(_str(4));
vreg_7 = vreg_1->side;
CodeRefactor_OpenRuntime_CrConsole_WriteLine(vreg_7);
CodeRefactor_OpenRuntime_CrConsole_Write(_str(5));
vreg_8 = vreg_3->side;
CodeRefactor_OpenRuntime_CrConsole_WriteLine(vreg_8);
return;
}


System_Void _Square_ctor(const std::shared_ptr<_Square>& _this, System_Double s)

{

_this->side = s;
return;
}


System_Void _Cube_ctor(const std::shared_ptr<_Cube>& _this, System_Double s)

{

_this->side = s;
return;
}


System_Void _Shape_ctor()

{

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
_AddJumpAndLength(0, 16);
_AddJumpAndLength(17, 20);
_AddJumpAndLength(38, 18);
_AddJumpAndLength(57, 16);
_AddJumpAndLength(74, 21);
_AddJumpAndLength(96, 19);
} // buildStringTable
const wchar_t _stringTable[116] = {
69, 110, 116, 101, 114, 32, 116, 104, 101, 32, 115, 105, 100, 101, 58, 32, 0 /* "Enter the side: " */, 
65, 114, 101, 97, 32, 111, 102, 32, 116, 104, 101, 32, 115, 113, 117, 97, 114, 101, 32, 61, 0 /* "Area of the square =" */, 
65, 114, 101, 97, 32, 111, 102, 32, 116, 104, 101, 32, 99, 117, 98, 101, 32, 61, 0 /* "Area of the cube =" */, 
69, 110, 116, 101, 114, 32, 116, 104, 101, 32, 97, 114, 101, 97, 58, 32, 0 /* "Enter the area: " */, 
83, 105, 100, 101, 32, 111, 102, 32, 116, 104, 101, 32, 115, 113, 117, 97, 114, 101, 32, 61, 32, 0 /* "Side of the square = " */, 
83, 105, 100, 101, 32, 111, 102, 32, 116, 104, 101, 32, 99, 117, 98, 101, 32, 61, 32, 0 /* "Side of the cube = " */
}; // _stringTable 

