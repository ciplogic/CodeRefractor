#include "sloth.h"
struct SimpleAdditions_NBody; 
struct System_Console; 
struct SimpleAdditions_NBody {
};
struct System_Console {
};
System_Void SimpleAdditions_NBody_Main();

System_Void System_Console_WriteLine(System_Int32 value);

#include "runtime_base.partcpp"
#include "stdio.h"
System_Void System_Console_WriteLine(System_Int32 value)
{ printf("%d\n", value); }
///---Begin closure code --- 
System_Void SimpleAdditions_NBody_Main()
{

System_Console_WriteLine(4);
return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
SimpleAdditions_NBody_Main();
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

