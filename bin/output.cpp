#include "sloth.h"
struct System_Object; 
struct Game_App; 
struct Game_Base; 
struct Game_ImplBase; 
struct Game_ImplBaseB; 
struct System_Console; 
struct System_String; 
struct System_Object {
int _typeId;
};
struct Game_App : public System_Object {
};
struct Game_Base : public System_Object {
};
struct Game_ImplBase : public Game_Base {
};
struct Game_ImplBaseB : public Game_Base {
};
struct System_Console : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};

System_Void Game_App_Main();

System_Void Game_ImplBase_ctor(Game_ImplBase * _this);

System_Void Game_ImplBaseB_ctor(Game_ImplBaseB * _this);

System_Void Game_ImplBase_InterfaceMethod(Game_ImplBase * _this);

System_Void Game_ImplBase_ToImplement(Game_ImplBase * _this);

System_Void Game_ImplBaseB_ToImplement(Game_ImplBaseB * _this);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

typedef void (*Game_Base_ToImplementVirtPtr)(const std::shared_ptr<Game_Base> &_this);
void Game_Base_ToImplement_vcall(const std::shared_ptr<Game_Base> &_this);
typedef void (*Game_ImplBase_InterfaceMethodVirtPtr)(const std::shared_ptr<Game_ImplBase> &_this);
void Game_ImplBase_InterfaceMethod_vcall(const std::shared_ptr<Game_ImplBase> &_this);
int virt_typeId_Game_Base_ToImplement[2];
Game_Base_ToImplementVirtPtr methods_Game_Base_ToImplement[2];
void Game_Base_ToImplement_vcall(const std::shared_ptr<Game_Base> &_this){
switch(_this->_typeId)
{
case 3:
Game_ImplBase_ToImplement(_this);
return;
case 4:
Game_ImplBaseB_ToImplement(_this);
return;
}
}
int virt_typeId_Game_ImplBase_InterfaceMethod[1];
Game_ImplBase_InterfaceMethodVirtPtr methods_Game_ImplBase_InterfaceMethod[1];
void Game_ImplBase_InterfaceMethod_vcall(const std::shared_ptr<Game_ImplBase> &_this){
switch(_this->_typeId)
{
case 3:
Game_ImplBase_InterfaceMethod(_this);
return;
}
}
// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void Game_App_Main()

{
Game_ImplBase * local_1;
Array < std::shared_ptr<Game_Base> > * local_4;
System_Int32 local_5;
std::shared_ptr<Game_ImplBase> vreg_2;
std::shared_ptr<Game_ImplBaseB> vreg_4;
Game_Base * vreg_6;
System_Int32 vreg_8;
System_Int32 vreg_9;

Array <std::shared_ptr<Game_Base>> vreg_1 (2); 
vreg_2 = std::make_shared<Game_ImplBase >();
vreg_2->_typeId = 3;
Game_ImplBase_ctor(vreg_2.get());
local_1 = (vreg_2).get();
vreg_1[0] = vreg_2; 
vreg_4 = std::make_shared<Game_ImplBaseB >();
vreg_4->_typeId = 4;
Game_ImplBaseB_ctor(vreg_4.get());
vreg_1[1] = vreg_4; 
local_4 = &vreg_1;
local_5 = 0;
vreg_8 = local_4->Length;
vreg_9 = (int)vreg_8;
goto label_60;
label_40:
vreg_6 = ((*local_4)[local_5]).get();
Game_Base_ToImplement_vcall(vreg_6);
local_5 = local_5+1;
label_60:
if(vreg_9<local_5) goto label_40;
Game_ImplBase_InterfaceMethod_vcall(local_1);
return;
}


System_Void Game_ImplBase_ctor(Game_ImplBase * _this)

{

return;
}


System_Void Game_ImplBaseB_ctor(Game_ImplBaseB * _this)

{

return;
}


System_Void Game_ImplBase_InterfaceMethod(Game_ImplBase * _this)

{

System_Console_WriteLine(_str(0));
return;
}


System_Void Game_ImplBase_ToImplement(Game_ImplBase * _this)

{

System_Console_WriteLine(_str(1));
return;
}


System_Void Game_ImplBaseB_ToImplement(Game_ImplBaseB * _this)

{

System_Console_WriteLine(_str(2));
return;
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
_AddJumpAndLength(0, 23);
_AddJumpAndLength(24, 8);
_AddJumpAndLength(33, 9);
} // buildStringTable
const wchar_t _stringTable[43] = {
73, 109, 112, 108, 66, 97, 115, 101, 46, 73, 66, 97, 115, 101, 73, 110, 116, 101, 114, 102, 97, 99, 101, 0 /* "ImplBase.IBaseInterface" */, 
73, 109, 112, 108, 66, 97, 115, 101, 0 /* "ImplBase" */, 
73, 109, 112, 108, 66, 97, 115, 101, 66, 0 /* "ImplBaseB" */
}; // _stringTable 

void setupTypeTable(){
virt_typeId_Game_Base_ToImplement[0] = 3;
methods_Game_Base_ToImplement[0] = (Game_Base_ToImplementVirtPtr)Game_ImplBase_ToImplement;
virt_typeId_Game_Base_ToImplement[1] = 4;
methods_Game_Base_ToImplement[1] = (Game_Base_ToImplementVirtPtr)Game_ImplBaseB_ToImplement;
virt_typeId_Game_ImplBase_InterfaceMethod[0] = 3;
methods_Game_ImplBase_InterfaceMethod[0] = (Game_ImplBase_InterfaceMethodVirtPtr)Game_ImplBase_InterfaceMethod;
}

