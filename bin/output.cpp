#include "sloth.h"
#include <functional>
struct System_Object;
struct CodeRefactor_OpenRuntime_CrString;
struct _Program;
struct IDisp_PersistentData;
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

struct IDisp_PersistentData : public System_Object {
    System_Int32 enabled;
};

struct System_Console : public System_Object {
};

System_Void _Program_Main();
System_Void IDisp_PersistentData_ctor(const std::shared_ptr<IDisp_PersistentData>& _this);
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value);
System_Void IDisp_PersistentData_Dispose(const std::shared_ptr<IDisp_PersistentData>& _this);

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
    std::shared_ptr<IDisp_PersistentData> local_0;
    System_Boolean local_1;
    std::shared_ptr<IDisp_PersistentData> vreg_1;
    
    vreg_1 = std::make_shared<IDisp_PersistentData >();vreg_1->_typeId = 3;
    IDisp_PersistentData_ctor(vreg_1);
    local_0 = (vreg_1);
    System_Console_WriteLine(_str(0));
    local_1 = (vreg_1 == nullptr)?1:0;
    if(local_1) goto label_25;
    IDisp_PersistentData_Dispose(local_0);
    label_25:
	return;

}
System_Void IDisp_PersistentData_ctor(const std::shared_ptr<IDisp_PersistentData>& _this) {
    
    _this->enabled = 1;
    System_Console_WriteLine(_str(1));

}
System_Void IDisp_PersistentData_Dispose(const std::shared_ptr<IDisp_PersistentData>& _this) {
    
    _this->enabled = 0;
    System_Console_WriteLine(_str(2));

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
    _AddJumpAndLength(25, 11);
} // buildStringTable

const System_Char _stringTable[37] = {
    117, 115, 105, 110, 103, 32, 112, 101, 114, 115, 105, 115, 116, 101, 110, 116, 0 /* "using persistent" */, 
    69, 110, 97, 98, 108, 101, 100, 0 /* "Enabled" */, 
    78, 111, 116, 32, 101, 110, 97, 98, 108, 101, 100, 0 /* "Not enabled" */
}; // _stringTable
