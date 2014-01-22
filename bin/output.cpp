#include "sloth.h"
struct System_ValueType; 
struct System_Array; 
struct System_String; 
struct System_Object; 
struct System_Text_StringBuilder; 
struct SimpleAdditions_Program; 
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
struct SimpleAdditions_Program {
};

System_Void SimpleAdditions_Program_Main();

#include "runtime_base.partcpp"
///---Begin closure code --- 
System_Void SimpleAdditions_Program_Main()
{
std::shared_ptr< Array < std::shared_ptr<System_String> > > vreg_5;
std::shared_ptr< Array < std::shared_ptr<System_String> > >*  vreg_13;

vreg_5 = std::make_shared< Array <std::shared_ptr<System_String>> >(2); 
(*vreg_5)[0] = _str(0); 
(*vreg_5)[1] = _str(1); 
vreg_13 = &vreg_5;
System_Array_Resize(vreg_13, 3);
(*vreg_5)[2] = _str(2); 
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
_AddJumpAndLength(0, 3);
_AddJumpAndLength(4, 3);
_AddJumpAndLength(8, 5);
} // buildStringTable
const wchar_t _stringTable[14] = {
111, 110, 101, 0 /* "one" */, 
116, 119, 111, 0 /* "two" */, 
116, 104, 114, 101, 101, 0 /* "three" */
}; // _stringTable 

