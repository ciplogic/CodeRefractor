#include "sloth.h"

struct System_Object;
struct CodeRefactor_OpenRuntime_CrString;
struct _TestShapes;
struct CodeRefactor_OpenRuntime_CrConsole;
struct _Shape;
struct _Square;
struct _Cube;
struct CodeRefactor_OpenRuntime_CrMath;
struct System_Object {
    int _typeId;
};

struct System_String : public System_Object {
    System_String() {
        _typeId = 11;
    }

    std::shared_ptr< Array < System_Char > > Text;
};

struct _TestShapes : public System_Object {
};

struct System_Console : public System_Object {
};

struct _Shape : public System_Object {
};

struct _Square : public _Shape {
    System_Double side;
};

struct _Cube : public _Shape {
    System_Double side;
};

struct System_Math : public System_Object {
};

System_Void _TestShapes_Main();
System_Void System_Console_Write(std::shared_ptr<System_String> value);
System_Void System_Console_WriteLine(System_Double value);
System_Void System_Console_WriteLine(System_Single value);
System_Double _Square_get_Area(_Square* _this);
System_Double _Cube_get_Area(_Cube* _this);
System_Void _Square_set_Area(_Square* _this, System_Double value);
System_Void _Cube_set_Area(_Cube* _this, System_Double value);
System_Void System_Console_Write(System_Double value);
System_Void System_Console_WriteLine();
System_Void System_Console_Write(System_Single value);
System_Double System_Math_Sqrt(System_Double d);

#include "runtime_base.hpp"

System_Void buildTypesTable() {
}

bool IsInstanceOf(int typeSource, int typeImplementation) {
    auto typeVector = GlobalMappingType[typeSource];
	auto begin = typeVector.begin();
	auto end = typeVector.end();
	return std::find(begin, end, typeImplementation)!= end;

}


// --- Begin definition of virtual implementingMethod tables ---
System_Void setupTypeTable();

#include "stdio.h"
System_Void System_Console_Write(std::shared_ptr<System_String> value) {
    printf("%ls", value.get()->Text->Items);
}
System_Void System_Console_Write(System_Double value) {
    
        //Borrowed from SharpLang
        char buffer[64];
    	int length = sprintf(buffer, "%.14f", value); // Match default .Net precision, makes testing easier
    
    	// Remove trailing 0 (after .)
    	if (strchr(buffer, '.') != NULL)
    	{
    		while (length > 0 && buffer[length - 1] == '0')
    			buffer[--length] = '\0';
    	}
    
    	if (length > 0 && buffer[length - 1] == '.')
    		buffer[--length] = '\0';
    
    	printf("%s", buffer);
}
System_Void System_Console_WriteLine() {
    printf("\n");
}
System_Void System_Console_Write(System_Single value) {
    
        //Borrowed from SharpLang
        char buffer[64];
    	int length = sprintf(buffer, "%.6f", value);  // Match default .Net precision, makes testing easier
    
    	// Remove trailing 0 (after .)
    	if (strchr(buffer, '.') != NULL)
    	{
    		while (length > 0 && buffer[length - 1] == '0')
    			buffer[--length] = '\0';
    	}
    
    	if (length > 0 && buffer[length - 1] == '.')
    		buffer[--length] = '\0';
    
    	printf("%s", buffer);
}
#include "math.h"
System_Double System_Math_Sqrt(System_Double d) {
    return sqrt(d);
}
///--- PInvoke code ---
///---Begin closure code ---
System_Void _TestShapes_Main() {
    System_Double vreg_5;
    System_Double vreg_6;
    System_Double vreg_7;
    System_Single vreg_8;
    System_Double vreg_9;
    System_Single vreg_10;
    
    System_Console_Write(_str(0));
    _Square  vreg_1;vreg_1._typeId = 4;
    _Cube  vreg_3;vreg_3._typeId = 6;
    System_Console_Write(_str(1));
    vreg_5 = _Square_get_Area(&vreg_1);
    System_Console_WriteLine(vreg_5);
    System_Console_Write(_str(2));
    vreg_6 = _Cube_get_Area(&vreg_3);
    System_Console_WriteLine(vreg_6);
    System_Console_Write(_str(3));
    _Square_set_Area(&vreg_1, 50);
    _Cube_set_Area(&vreg_3, 50);
    System_Console_Write(_str(4));
    vreg_7 = vreg_1.side;
    vreg_8 = (float)vreg_7;
    System_Console_WriteLine(vreg_8);
    System_Console_Write(_str(5));
    vreg_9 = vreg_3.side;
    vreg_10 = (float)vreg_9;
    System_Console_WriteLine(vreg_10);

    return;
}
System_Void System_Console_WriteLine(System_Double value) {
    
    System_Console_Write(value);
    System_Console_WriteLine();

    return;
}
System_Void System_Console_WriteLine(System_Single value) {
    
    System_Console_Write(value);
    System_Console_WriteLine();

    return;
}
System_Double _Square_get_Area(_Square* _this) {
    System_Double local_0;
    System_Double vreg_3;
    
    vreg_3 = _this->side;
    local_0 = vreg_3*vreg_3;

    return local_0;
}
System_Double _Cube_get_Area(_Cube* _this) {
    System_Double local_0;
    System_Double vreg_2;
    System_Double vreg_4;
    
    vreg_4 = _this->side;
    vreg_2 = 6*vreg_4;
    local_0 = vreg_2*vreg_4;

    return local_0;
}
System_Void _Square_set_Area(_Square* _this, System_Double value) {
    System_Double vreg_1;
    
    vreg_1 = System_Math_Sqrt(value);
    _this->side = vreg_1;

    return;
}
System_Void _Cube_set_Area(_Cube* _this, System_Double value) {
    System_Double vreg_1;
    System_Double vreg_2;
    
    vreg_1 = value/6;
    vreg_2 = System_Math_Sqrt(vreg_1);
    _this->side = vreg_2;

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
    _AddJumpAndLength(0, 16);
    _AddJumpAndLength(17, 20);
    _AddJumpAndLength(38, 18);
    _AddJumpAndLength(57, 16);
    _AddJumpAndLength(74, 21);
    _AddJumpAndLength(96, 19);
} // buildStringTable

const System_Char _stringTable[116] = {
    69, 110, 116, 101, 114, 32, 116, 104, 101, 32, 115, 105, 100, 101, 58, 32, 0 /* "Enter the side: " */, 
    65, 114, 101, 97, 32, 111, 102, 32, 116, 104, 101, 32, 115, 113, 117, 97, 114, 101, 32, 61, 0 /* "Area of the square =" */, 
    65, 114, 101, 97, 32, 111, 102, 32, 116, 104, 101, 32, 99, 117, 98, 101, 32, 61, 0 /* "Area of the cube =" */, 
    69, 110, 116, 101, 114, 32, 116, 104, 101, 32, 97, 114, 101, 97, 58, 32, 0 /* "Enter the area: " */, 
    83, 105, 100, 101, 32, 111, 102, 32, 116, 104, 101, 32, 115, 113, 117, 97, 114, 101, 32, 61, 32, 0 /* "Side of the square = " */, 
    83, 105, 100, 101, 32, 111, 102, 32, 116, 104, 101, 32, 99, 117, 98, 101, 32, 61, 32, 0 /* "Side of the cube = " */
}; // _stringTable
