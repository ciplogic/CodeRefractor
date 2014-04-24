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

System_Void CodeRefactor_OpenRuntime_CrConsole_WriteLine(std::shared_ptr<System_String> value);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

// --- End of definition of virtual method tables ---

///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{

System_Console_WriteLine(test);
return;
}


System_Void CodeRefactor_OpenRuntime_CrConsole_WriteLine(std::shared_ptr<System_String> value)

{

return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
_NBody_Main(argsAsList);
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

