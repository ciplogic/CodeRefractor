#include "sloth.h"

struct System_Object;
struct CodeRefactor_OpenRuntime_CrString;
struct _Test;
struct CodeRefactor_OpenRuntime_StringImpl;
struct CodeRefactor_OpenRuntime_CrConsole;
struct System_Object {
    int _typeId;
};

struct System_String : public System_Object {
    System_String() {
        _typeId = 7;
    }

    std::shared_ptr< Array < System_Char > > Text;
};

struct _Test : public System_Object {
};

struct CodeRefactor_OpenRuntime_StringImpl : public System_Object {
};

struct System_Console : public System_Object {
};

System_Void _Test_Main();
std::shared_ptr<System_String> CodeRefactor_OpenRuntime_StringImpl_Concat(std::shared_ptr<System_String> s1, std::shared_ptr<System_String> s2);
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
System_Void _Test_Main() {
    std::shared_ptr<System_String> vreg_1;
    
    vreg_1 = CodeRefactor_OpenRuntime_StringImpl_Concat(_str(0), _str(1));
    System_Console_WriteLine(vreg_1);

    return;
}
std::shared_ptr<System_String> CodeRefactor_OpenRuntime_StringImpl_Concat(std::shared_ptr<System_String> s1, std::shared_ptr<System_String> s2) {
    Array < System_Char >  * local_0;
    Array < System_Char >  * local_1;
    std::shared_ptr< Array < System_Char > > local_2;
    System_Int32 local_3;
    System_Int32 local_4;
    System_Boolean local_6;
    std::shared_ptr< Array < System_Char > > vreg_1;
    std::shared_ptr< Array < System_Char > > vreg_2;
    System_Int32 vreg_5;
    System_Int32 vreg_6;
    System_Int32 vreg_7;
    System_Int32 vreg_8;
    std::shared_ptr< Array < System_Char > > vreg_9;
    System_Char vreg_10;
    System_Int32 vreg_13;
    System_Int32 vreg_15;
    System_Int32 vreg_16;
    System_Int32 vreg_17;
    System_Char vreg_18;
    System_Int32 vreg_20;
    System_Int32 vreg_21;
    std::shared_ptr<System_String> vreg_23;
    System_Int32 vreg_24;
    System_Int32 vreg_25;
    
    vreg_1 = System_String_ToCharArray(s1);
    local_0 = (vreg_1).get();
    vreg_2 = System_String_ToCharArray(s2);
    local_1 = (vreg_2).get();
    vreg_24 = vreg_1->Length;
    vreg_25 = (int)vreg_24;
    vreg_5 = vreg_2->Length;
    vreg_6 = (int)vreg_5;
    vreg_7 = vreg_25+vreg_6;
    vreg_8 = vreg_7+1;
    vreg_9 = std::make_shared< Array <System_Char> >(vreg_8); 
    local_2 = vreg_9;
    local_3 = 0;
    vreg_13 = vreg_25;
    goto label_2E;
    label_22:
    vreg_10 = (*local_0)[local_3];
    (*local_2)[local_3] = vreg_10; 
    local_3 = local_3+1;
    label_2E:
    local_6 = (local_3 < vreg_13)?1:0;
    if(local_6) goto label_22;
    vreg_15 = local_0->Length;
    vreg_16 = (int)vreg_15;
    local_4 = vreg_16;
    local_3 = 0;
    vreg_20 = local_1->Length;
    vreg_21 = (int)vreg_20;
    goto label_52;
    label_43:
    vreg_17 = local_3+local_4;
    vreg_18 = (*local_1)[local_3];
    (*local_2)[vreg_17] = vreg_18; 
    local_3 = local_3+1;
    label_52:
    local_6 = (local_3 < vreg_21)?1:0;
    if(local_6) goto label_43;
    vreg_23 = std::make_shared<System_String >();vreg_23->_typeId = 7;
    System_String_ctor(vreg_23, local_2);

    return vreg_23;
}
///---End closure code ---
System_Void initializeRuntime();
int main(int argc, char**argv) {
    auto argsAsList = System_getArgumentsAsList(argc, argv);
    initializeRuntime();
    _Test_Main();

    return 0;
}

System_Void mapLibs() {
}


System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
    _AddJumpAndLength(0, 3);
    _AddJumpAndLength(4, 3);
} // buildStringTable

const System_Char _stringTable[8] = {
    97, 98, 99, 0 /* "abc" */, 
    100, 101, 102, 0 /* "def" */
}; // _stringTable
