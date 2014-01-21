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
Array <std::shared_ptr<System_String>> *  vreg_13;

Array <std::shared_ptr<System_String>> vreg_5 (2); 
(vreg_5)[0] = one; 
(vreg_5)[1] = two; 
vreg_13 = &vreg_5;
System_Array_Resize (vreg_13, 3);
(vreg_5)[2] = three; 
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
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

