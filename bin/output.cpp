#include "sloth.h"
#include <functional>
struct System_Object; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct _TestShapes; 
struct CodeRefactor_OpenRuntime_CrString; 
struct System_Object {
int _typeId;
};
struct System_Console : public System_Object {
};
struct _TestShapes : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};

System_Void _TestShapes_Main();

System_Double System_Double_Parse(std::shared_ptr<System_String> text);

System_Void System_Console_Write(System_Double value);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Double System_Double_Parse(std::shared_ptr<System_String> text)
{ System_Double result; swscanf_s(text->Text->Items, L"%lf", &result); return result; }
System_Void System_Console_Write(System_Double value)
{ printf("%lf", value); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _TestShapes_Main()
{
System_Double vreg_1;

vreg_1 = System_Double_Parse(_str(0));
System_Console_Write(vreg_1);
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
_AddJumpAndLength(0, 3);
} // buildStringTable
const wchar_t _stringTable[4] = {
50, 46, 53, 0 /* "2.5" */
}; // _stringTable 

