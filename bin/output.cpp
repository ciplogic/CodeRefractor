#include "sloth.h"
struct System_Object; 
struct System_Console; 
struct Game_App; 
struct Game_Base; 
struct System_String; 
struct Game_ImplBaseB; 
struct Game_ImplBase; 
struct System_Object {
int _typeId;
};
struct System_Console : public System_Object {
};
struct Game_App : public System_Object {
};
struct Game_Base : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct Game_ImplBaseB : public Game_Base {
};
struct Game_ImplBase : public Game_Base {
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
int typeId = _this->_typeId;
for(int i= 0; i<2;i++)
{
if(virt_typeId_Game_Base_ToImplement[i] == typeId)
methods_Game_Base_ToImplement[i](_this);
}
}
int virt_typeId_Game_ImplBase_InterfaceMethod[1];
Game_ImplBase_InterfaceMethodVirtPtr methods_Game_ImplBase_InterfaceMethod[1];
void Game_ImplBase_InterfaceMethod_vcall(const std::shared_ptr<Game_ImplBase> &_this){
int typeId = _this->_typeId;
for(int i= 0; i<1;i++)
{
if(virt_typeId_Game_ImplBase_InterfaceMethod[i] == typeId)
methods_Game_ImplBase_InterfaceMethod[i](_this);
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
std::shared_ptr<Game_ImplBase> vreg_3;
std::shared_ptr<Game_ImplBaseB> vreg_8;
std::shared_ptr<Game_Base> vreg_17;
System_Int32 vreg_24;
System_Int32 vreg_25;
System_Boolean vreg_27;

Array <std::shared_ptr<Game_Base>> vreg_2 (2); 
vreg_3 = std::make_shared<Game_ImplBase >();
vreg_3->_typeId = 2;
Game_ImplBase_ctor(vreg_3.get());
local_1 = (vreg_3).get();
vreg_2[0] = vreg_3; 
vreg_8 = std::make_shared<Game_ImplBaseB >();
vreg_8->_typeId = 1;
Game_ImplBaseB_ctor(vreg_8.get());
vreg_2[1] = vreg_8; 
local_4 = &vreg_2;
local_5 = 0;
vreg_24 = local_4->Length;
vreg_25 = (int)vreg_24;
goto label_58;
label_37:
vreg_17 = (*local_4)[local_5];
Game_Base_ToImplement_vcall(vreg_17);
local_5 = local_5+1;
label_58:
vreg_27 = (local_5 < vreg_25)?1:0;
if(vreg_27) goto label_37;
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
virt_typeId_Game_Base_ToImplement[0] = 1;
methods_Game_Base_ToImplement[0] = (Game_Base_ToImplementVirtPtr)Game_ImplBaseB_ToImplement;
virt_typeId_Game_Base_ToImplement[1] = 2;
methods_Game_Base_ToImplement[1] = (Game_Base_ToImplementVirtPtr)Game_ImplBase_ToImplement;
virt_typeId_Game_ImplBase_InterfaceMethod[0] = 2;
methods_Game_ImplBase_InterfaceMethod[0] = (Game_ImplBase_InterfaceMethodVirtPtr)Game_ImplBase_InterfaceMethod;
}

