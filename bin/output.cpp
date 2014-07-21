#include "sloth.h"
#include <functional>
struct System_Object; 
struct System_ValueType; 
struct _Shape; 
struct CodeRefactor_OpenRuntime_CrMath; 
struct _Square; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _TestShapes; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct _Cube; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};
struct _Shape : public System_Object {
};
struct System_Math : public System_Object {
};
struct _Square : public _Shape {
 System_Double side;
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct _TestShapes : public System_Object {
};
struct System_Console : public System_Object {
};
struct _Cube : public _Shape {
 System_Double side;
};

System_Void _TestShapes_Main();

System_Void System_Console_Write(std::shared_ptr<System_String> value);

System_Void _Square_ctor(const std::shared_ptr<_Square>& _this, System_Double s);

System_Void _Cube_ctor(const std::shared_ptr<_Cube>& _this, System_Double s);

System_Void System_Console_WriteLine(System_Double value);

System_Double _Square_get_Area(const std::shared_ptr<_Square>& _this);

System_Double _Cube_get_Area(const std::shared_ptr<_Cube>& _this);

System_Void _Square_set_Area(const std::shared_ptr<_Square>& _this, System_Double value);

System_Void _Cube_set_Area(const std::shared_ptr<_Cube>& _this, System_Double value);

System_Void _Shape_ctor();

System_Double System_Math_Sqrt(System_Double d);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

System_Double _Shape_get_Area_vcall(const std::shared_ptr<_Shape> _this);
System_Void _Shape_set_Area_vcall(const std::shared_ptr<_Shape> _this, System_Double value);

#include "stdio.h"
System_Void System_Console_Write(std::shared_ptr<System_String> value)
{
printf("%ls", value.get()->Text->Items);
}
System_Void System_Console_WriteLine(System_Double value)
{
printf("%lf\n", value);
}
#include "math.h"
System_Double System_Math_Sqrt(System_Double d)
{
return sqrt(d);
}
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

System_Console_Write(_str(0));
vreg_1 = std::make_shared<_Square >();
vreg_1->_typeId = 4;
vreg_3 = std::make_shared<_Cube >();
vreg_3->_typeId = 6;
System_Console_Write(_str(1));
vreg_5 = _Shape_get_Area_vcall(vreg_1);
System_Console_WriteLine(vreg_5);
System_Console_Write(_str(2));
vreg_6 = _Shape_get_Area_vcall(vreg_3);
System_Console_WriteLine(vreg_6);
System_Console_Write(_str(3));
_Shape_set_Area_vcall(vreg_1, 50);
_Shape_set_Area_vcall(vreg_3, 50);
System_Console_Write(_str(4));
vreg_7 = vreg_1->side;
System_Console_WriteLine(vreg_7);
System_Console_Write(_str(5));
vreg_8 = vreg_3->side;
System_Console_WriteLine(vreg_8);
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


System_Double _Square_get_Area(const std::shared_ptr<_Square>& _this)
{
System_Double local_0;
System_Double vreg_3;

vreg_3 = _this->side;
local_0 = vreg_3*vreg_3;
return local_0;
}


System_Double _Cube_get_Area(const std::shared_ptr<_Cube>& _this)
{
System_Double local_0;
System_Double vreg_2;
System_Double vreg_4;

vreg_4 = _this->side;
vreg_2 = 6*vreg_4;
local_0 = vreg_2*vreg_4;
return local_0;
}


System_Void _Square_set_Area(const std::shared_ptr<_Square>& _this, System_Double value)
{
System_Double vreg_1;

vreg_1 = System_Math_Sqrt(value);
_this->side = vreg_1;
return;
}


System_Void _Cube_set_Area(const std::shared_ptr<_Cube>& _this, System_Double value)
{
System_Double vreg_1;
System_Double vreg_2;

vreg_1 = value/6;
vreg_2 = System_Math_Sqrt(vreg_1);
_this->side = vreg_2;
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

