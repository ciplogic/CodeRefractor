#include "sloth.h"
struct System_Object; 
struct System_ValueType; 
struct _NBody; 
struct System_String; 
struct System_Console; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};
struct _NBody : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct System_Console : public System_Object {
};

System_Void _NBody_Main();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{

System_Console_WriteLine(_str(0));
return;
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
_AddJumpAndLength(0, 4);
} // buildStringTable
const wchar_t _stringTable[5] = {
116, 101, 115, 116, 0 /* "test" */
}; // _stringTable 

