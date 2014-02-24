#include "sloth.h"
struct System_Object; 
struct System_ValueType; 
struct Tao_Sdl_SDL_SysWMEvent; 
struct Tao_Sdl_SDL_UserEvent; 
struct Tao_Sdl_SDL_QuitEvent; 
struct Tao_Sdl_SDL_keysym; 
struct Tao_Sdl_SDL_ExposeEvent; 
struct Tao_Sdl_SDL_ResizeEvent; 
struct Tao_Sdl_SDL_JoyButtonEvent; 
struct Tao_Sdl_SDL_JoyHatEvent; 
struct Tao_Sdl_SDL_JoyBallEvent; 
struct Tao_Sdl_SDL_JoyAxisEvent; 
struct Tao_Sdl_SDL_ActiveEvent; 
struct Tao_Sdl_SDL_Event; 
struct Tao_Sdl_SDL_KeyboardEvent; 
struct Tao_Sdl_SDL_MouseButtonEvent; 
struct Tao_Sdl_SDL_MouseMotionEvent; 
struct SimpleAdditions_CrSdl; 
struct System_Console; 
struct Game_GameClass; 
struct Game_App; 
struct System_String; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};
struct Tao_Sdl_SDL_SysWMEvent {
};
struct Tao_Sdl_SDL_UserEvent {
};
struct Tao_Sdl_SDL_QuitEvent {
};
struct Tao_Sdl_SDL_keysym {
};
struct Tao_Sdl_SDL_ExposeEvent {
};
struct Tao_Sdl_SDL_ResizeEvent {
};
struct Tao_Sdl_SDL_JoyButtonEvent {
};
struct Tao_Sdl_SDL_JoyHatEvent {
};
struct Tao_Sdl_SDL_JoyBallEvent {
};
struct Tao_Sdl_SDL_JoyAxisEvent {
};
struct Tao_Sdl_SDL_ActiveEvent {
};
struct Tao_Sdl_SDL_Event {
};
struct Tao_Sdl_SDL_KeyboardEvent {
};
struct Tao_Sdl_SDL_MouseButtonEvent {
};
struct Tao_Sdl_SDL_MouseMotionEvent {
};
struct SimpleAdditions_CrSdl : public System_Object {
};
struct System_Console : public System_Object {
};
struct Game_GameClass : public System_Object {
 System_Boolean fin;
 Tao_Sdl_SDL_Event e;
 System_Single xrot;
 System_Single yrot;
};
struct Game_App : public System_Object {
 std::shared_ptr<Game_GameClass> game;
 System_Int32 w;
 System_Int32 h;
 System_Int32 bpp;
 System_Int32 flags;
static System_Single rtri;
static System_Single rquad;
};
 /* static*/ System_Single Game_App::rtri = 0;
 /* static*/ System_Single Game_App::rquad = 0;
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};

System_Void Game_App_Main();

System_Void Game_App_ctor(Game_App * _this);

System_Boolean Game_GameClass_get_finP(Game_GameClass * _this);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
///--- PInvoke code --- 
typedef System_Int32 (__cdecl *dll_method_1_type)(System_Int32 flags);
dll_method_1_type dll_method_1;
System_Int32 Tao_Sdl_Sdl_SDL_Init(System_Int32 flags)
{
return dll_method_1(flags);
}

typedef void (__cdecl *dll_method_2_type)();
dll_method_2_type dll_method_2;
System_Void Tao_Sdl_Sdl_SDL_Quit()
{
dll_method_2();
}

typedef void (__cdecl *dll_method_3_type)(System_Int32 ms);
dll_method_3_type dll_method_3;
System_Void Tao_Sdl_Sdl_SDL_Delay(System_Int32 ms)
{
dll_method_3(ms);
}

///---Begin closure code --- 
System_Void Game_App_Main()

{

Game_App  vreg_1;
vreg_1._typeId = 21;
Game_App_ctor(&vreg_1);
return;
}


System_Void Game_App_ctor(Game_App * _this)

{
System_Boolean local_0;
System_Int32 vreg_1;
System_Int32 vreg_2;
std::shared_ptr<Game_GameClass> vreg_4;
System_Boolean vreg_8;
Game_GameClass * vreg_9;

_this->w = 1024;
_this->h = 768;
_this->bpp = 16;
_this->flags = 7;
vreg_1 = Tao_Sdl_Sdl_SDL_Init(32);
vreg_2 = (vreg_1 < 0)?1:0;
local_0 = (vreg_2 == 0)?1:0;
if(local_0) goto label_82;
System_Console_WriteLine(_str(0));
Tao_Sdl_Sdl_SDL_Quit();
goto label_155;
label_82:
vreg_4 = std::make_shared<Game_GameClass >();
vreg_4->_typeId = 20;
_this->game = vreg_4;
vreg_9 = _this->game.get();
vreg_8 = Game_GameClass_get_finP(vreg_9);
local_0 = (vreg_8 == 0)?1:0;
goto label_129;
label_102:
Tao_Sdl_Sdl_SDL_Delay(1);
label_129:
if(local_0) goto label_102;
Tao_Sdl_Sdl_SDL_Quit();
label_155:
return;
}


System_Boolean Game_GameClass_get_finP(Game_GameClass * _this)

{
System_Boolean local_0;
System_Boolean vreg_1;

vreg_1 = _this->fin;
local_0 = vreg_1;
goto label_10;
label_10:
return local_0;
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
auto lib_0 = LoadNativeLibrary(L"sdl.dll");
dll_method_1 = (dll_method_1_type)LoadNativeMethod(lib_0, "SDL_Init");
dll_method_2 = (dll_method_2_type)LoadNativeMethod(lib_0, "SDL_Quit");
dll_method_3 = (dll_method_3_type)LoadNativeMethod(lib_0, "SDL_Delay");
}

void RuntimeHelpersBuildConstantTable() {
}

void buildStringTable() {
_AddJumpAndLength(0, 22);
} // buildStringTable
const wchar_t _stringTable[23] = {
69, 114, 114, 111, 114, 32, 105, 110, 105, 116, 105, 97, 108, 105, 122, 105, 110, 103, 32, 83, 68, 76, 0 /* "Error initializing SDL" */
}; // _stringTable 

