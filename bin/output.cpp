#include "sloth.h"
struct System_Object; 
struct System_ValueType; 
struct SimpleAdditions_App; 
struct System_Console; 
struct System_String; 
struct SimpleAdditions_ImplBase; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};
struct SimpleAdditions_App : public System_Object {
};
struct System_Console : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct SimpleAdditions_ImplBase : public System_Object {
};

System_Void SimpleAdditions_App_Main();

System_Void SimpleAdditions_ImplBase_ToImplement();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void System_Console_WriteLine(System_Int32 value)
{ printf("%d\n", value); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void SimpleAdditions_App_Main()

{

SimpleAdditions_ImplBase_ToImplement();
return;
}


System_Void SimpleAdditions_ImplBase_ToImplement()

{

System_Console_WriteLine(2);
return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
SimpleAdditions_App_Main();
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

