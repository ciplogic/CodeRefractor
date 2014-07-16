#include "sloth.h"
#include <functional>
struct CodeRefactor_OpenRuntime_CrString; 
struct _NBody; 
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct _NBody : public System_Object {
};
template <class T1> 
struct _MyList_1System_Int32 : public System_Object {
};
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};

System_Void _NBody_Main();

System_Void _MyList_1System_Int32_ctor();

System_Void _MyList_1System_Int32_Add();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{

return;
}


System_Void _MyList_1System_Int32_ctor()

{

return;
}


System_Void _MyList_1System_Int32_Add()

{

return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System_getArgumentsAsList(argc, argv);
initializeRuntime();
_NBody_Main();
return 0;
}
System_Void mapLibs() {
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

