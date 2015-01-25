#include "sloth.h"
#include <functional>
struct System_Object;
struct CodeRefactor_OpenRuntime_CrString;
struct _Program;
struct _PersistentData;
struct CodeRefactor_OpenRuntime_CrConsole;
struct System_Object {
    int _typeId;
};

struct System_String : public System_Object {
    System_String() {
        _typeId = 6;
    }

    std::shared_ptr< Array < System_Char > > Text;
};

struct _Program : public System_Object {
};

struct _PersistentData : public System_Object {
    System_Int32 enabled;
};

struct System_Console : public System_Object {
};

System_Void _Program_Main();
System_Void _PersistentData_ctor(const std::shared_ptr<_PersistentData>& _this);
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value);

#include "runtime_base.hpp"

// --- Begin definition of virtual implementingMethod tables ---
System_Void setupTypeTable();

#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value) {
    printf("%ls\n", value.get()->Text->Items);
}
///--- PInvoke code ---
///---Begin closure code ---
System_Void _Program_Main() {
    std::shared_ptr<_PersistentData> vreg_1;
    
    vreg_1 = std::make_shared<_PersistentData >();vreg_1->_typeId = 3;
    _PersistentData_ctor(vreg_1);
    System_Console_WriteLine(_str(0));

}
System_Void _PersistentData_ctor(const std::shared_ptr<_PersistentData>& _this) {
    
    _this->enabled = 1;
    System_Console_WriteLine(_str(1));

}
///---End closure code ---
System_Void initializeRuntime();
int main(int argc, char**argv) {
    auto argsAsList = System_getArgumentsAsList(argc, argv);
    initializeRuntime();
    _Program_Main();

    return 0;
}

System_Void mapLibs() {
}


System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
    _AddJumpAndLength(0, 16);
    _AddJumpAndLength(17, 7);
} // buildStringTable

const System_Char _stringTable[25] = {
    117, 115, 105, 110, 103, 32, 112, 101, 114, 115, 105, 115, 116, 101, 110, 116, 0 /* "using persistent" */, 
    69, 110, 97, 98, 108, 101, 100, 0 /* "Enabled" */
}; // _stringTable
