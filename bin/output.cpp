#include "sloth.h"
struct Game_App; 
struct System_Object; 
struct Game_Base; 
struct Game_ImplBaseB; 
struct Game_ImplBase; 
struct System_ValueType; 
struct System_Console; 
struct System_IO_File; 
struct System_Array; 
struct System_Math; 
struct System_String; 
struct Game_App {
};
struct System_Object {
};
struct Game_Base {
};
struct Game_ImplBaseB : public Game_Base {
};
struct Game_ImplBase : public Game_Base {
};
struct System_ValueType {
};
struct System_Console {
};
struct System_IO_File {
};
struct System_Array {
};
struct System_Math {
};
struct System_String {
 std::shared_ptr< Array < System_Char > > Text;
};

System_Void Game_App_Main();

System_Void Game_ImplBase_ctor(const std::shared_ptr<Game_ImplBase>& _this);

System_Void Game_ImplBaseB_ctor(const std::shared_ptr<Game_ImplBaseB>& _this);

System_Void Game_Base_ToImplement(const std::shared_ptr<Game_Base>& _this);

#include "runtime_base.hpp"
///---Begin closure code --- 
System_Void Game_App_Main()

{
Array < std::shared_ptr<Game_Base> > * local_4;
System_Int32 local_5;
std::shared_ptr<Game_ImplBase> vreg_3;
std::shared_ptr<Game_ImplBaseB> vreg_8;
std::shared_ptr<Game_Base> vreg_17;
System_Int32 vreg_24;
System_Int32 vreg_25;
System_Boolean vreg_27;

Array <std::shared_ptr<Game_Base>> vreg_2 (2); 
vreg_3 = std::make_shared<Game_ImplBase >();

Game_ImplBase_ctor(vreg_3);
(vreg_2)[0] = vreg_3; 
vreg_8 = std::make_shared<Game_ImplBaseB >();

Game_ImplBaseB_ctor(vreg_8);
(vreg_2)[1] = vreg_8; 
local_4 = &vreg_2;
local_5 = 0;
vreg_24 = local_4->Length;
vreg_25 = (int)vreg_24;
goto label_58;
label_37:
vreg_17 = (*local_4)[local_5];
Game_Base_ToImplement(vreg_17);
local_5 = local_5+1;
label_58:
vreg_27 = (local_5 < vreg_25)?1:0;
if(vreg_27) goto label_37;
return;
}


System_Void Game_ImplBase_ctor(const std::shared_ptr<Game_ImplBase>& _this)

{

Game_ImplBase_ctor(_this);
return;
}


System_Void Game_ImplBaseB_ctor(const std::shared_ptr<Game_ImplBaseB>& _this)

{

Game_ImplBaseB_ctor(_this);
return;
}


System_Void Game_Base_ToImplement(const std::shared_ptr<Game_Base>& _this)

{

}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
Game_App_Main();
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

