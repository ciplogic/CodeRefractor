#include "sloth.h"
struct System_Object; 
struct System_ValueType; 
struct Tao_Sdl_SDL_JoyHatEvent; 
struct Tao_Sdl_SDL_JoyButtonEvent; 
struct Tao_Sdl_SDL_ResizeEvent; 
struct Tao_Sdl_SDL_ExposeEvent; 
struct Tao_Sdl_SDL_SysWMEvent; 
struct Tao_Sdl_SDL_keysym; 
struct Tao_Sdl_SDL_QuitEvent; 
struct Tao_Sdl_SDL_UserEvent; 
struct Tao_Sdl_SDL_KeyboardEvent; 
struct Tao_Sdl_SDL_ActiveEvent; 
struct Tao_Sdl_SDL_Event; 
struct Tao_Sdl_SDL_MouseMotionEvent; 
struct Tao_Sdl_SDL_JoyBallEvent; 
struct Tao_Sdl_SDL_JoyAxisEvent; 
struct Tao_Sdl_SDL_MouseButtonEvent; 
struct Game_App; 
struct Game_GameClass; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};
struct Tao_Sdl_SDL_JoyHatEvent {
 System_Byte type;
 System_Byte which;
 System_Byte hat;
 System_Byte val;
};
struct Tao_Sdl_SDL_JoyButtonEvent {
 System_Byte type;
 System_Byte which;
 System_Byte button;
 System_Byte state;
};
struct Tao_Sdl_SDL_ResizeEvent {
 System_Byte type;
 System_Int32 w;
 System_Int32 h;
};
struct Tao_Sdl_SDL_ExposeEvent {
 System_Byte type;
};
struct Tao_Sdl_SDL_SysWMEvent {
 System_Byte type;
 System_IntPtr msg;
};
struct Tao_Sdl_SDL_keysym {
 System_Byte scancode;
 System_Int32 sym;
 System_Int32 mod;
 System_Int16 unicode;
};
struct Tao_Sdl_SDL_QuitEvent {
 System_Byte type;
};
struct Tao_Sdl_SDL_UserEvent {
 System_Byte type;
 System_Int32 code;
 System_IntPtr data1;
 System_IntPtr data2;
};
struct Tao_Sdl_SDL_KeyboardEvent {
 System_Byte type;
 System_Byte which;
 System_Byte state;
 Tao_Sdl_SDL_keysym keysym;
};
struct Tao_Sdl_SDL_ActiveEvent {
 System_Byte type;
 System_Byte gain;
 System_Byte state;
};
struct Tao_Sdl_SDL_Event {
union {
 System_Byte type;
 Tao_Sdl_SDL_ActiveEvent active;
 Tao_Sdl_SDL_KeyboardEvent key;
 Tao_Sdl_SDL_MouseMotionEvent motion;
 Tao_Sdl_SDL_MouseButtonEvent button;
 Tao_Sdl_SDL_JoyAxisEvent jaxis;
 Tao_Sdl_SDL_JoyBallEvent jball;
 Tao_Sdl_SDL_JoyHatEvent jhat;
 Tao_Sdl_SDL_JoyButtonEvent jbutton;
 Tao_Sdl_SDL_ResizeEvent resize;
 Tao_Sdl_SDL_ExposeEvent expose;
 Tao_Sdl_SDL_QuitEvent quit;
 Tao_Sdl_SDL_UserEvent user;
 Tao_Sdl_SDL_SysWMEvent syswm;
};
};
struct Tao_Sdl_SDL_MouseMotionEvent {
 System_Byte type;
 System_Byte which;
 System_Byte state;
 System_Int16 x;
 System_Int16 y;
 System_Int16 xrel;
 System_Int16 yrel;
};
struct Tao_Sdl_SDL_JoyBallEvent {
 System_Byte type;
 System_Byte which;
 System_Byte ball;
 System_Int16 xrel;
 System_Int16 yrel;
};
struct Tao_Sdl_SDL_JoyAxisEvent {
 System_Byte type;
 System_Byte which;
 System_Byte axis;
 System_Int16 val;
};
struct Tao_Sdl_SDL_MouseButtonEvent {
 System_Byte type;
 System_Byte which;
 System_Byte button;
 System_Byte state;
 System_Int16 x;
 System_Int16 y;
};
struct Game_App : public System_Object {
 System_Int32 w;
 System_Int32 h;
 System_Int32 bpp;
 System_Int32 flags;
 std::shared_ptr<Game_GameClass> game;
static System_Single rtri;
static System_Single rquad;
};
 /* static*/ System_Single Game_App::rtri = 0;
 /* static*/ System_Single Game_App::rquad = 0;
struct Game_GameClass : public System_Object {
 System_Boolean fin;
 Tao_Sdl_SDL_Event e;
 System_Single xrot;
 System_Single yrot;
};
struct Tao_OpenGl_ShadeModel {
	std::vector< std::function<void(System_Int32)> > _functions;
void Register(std::function<void(System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
void Tao_OpenGl_ShadeModel_ctor(const std::shared_ptr<Tao_OpenGl_ShadeModel>& _delegate, void*, std::function<void(System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_ShadeModel_Invoke(const std::shared_ptr<Tao_OpenGl_ShadeModel>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct Tao_OpenGl_ClearColor {
	std::vector< std::function<void(System_Single,System_Single,System_Single,System_Single)> > _functions;
void Register(std::function<void(System_Single,System_Single,System_Single,System_Single) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Single arg0, System_Single arg1, System_Single arg2, System_Single arg3) {
for(auto it : _functions)
	(it)(arg0, arg1, arg2, arg3);
}
}; //end of class delegate
void Tao_OpenGl_ClearColor_ctor(const std::shared_ptr<Tao_OpenGl_ClearColor>& _delegate, void*, std::function<void(System_Single,System_Single,System_Single,System_Single)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_ClearColor_Invoke(const std::shared_ptr<Tao_OpenGl_ClearColor>& _delegate, System_Single arg0, System_Single arg1, System_Single arg2, System_Single arg3){
  _delegate->Invoke(arg0, arg1, arg2, arg3);
}
struct Tao_OpenGl_ClearDepth {
	std::vector< std::function<void(System_Double)> > _functions;
void Register(std::function<void(System_Double) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Double arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
void Tao_OpenGl_ClearDepth_ctor(const std::shared_ptr<Tao_OpenGl_ClearDepth>& _delegate, void*, std::function<void(System_Double)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_ClearDepth_Invoke(const std::shared_ptr<Tao_OpenGl_ClearDepth>& _delegate, System_Double arg0){
  _delegate->Invoke(arg0);
}
struct Tao_OpenGl_Enable {
	std::vector< std::function<void(System_Int32)> > _functions;
void Register(std::function<void(System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
void Tao_OpenGl_Enable_ctor(const std::shared_ptr<Tao_OpenGl_Enable>& _delegate, void*, std::function<void(System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Enable_Invoke(const std::shared_ptr<Tao_OpenGl_Enable>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct Tao_OpenGl_DepthFunc {
	std::vector< std::function<void(System_Int32)> > _functions;
void Register(std::function<void(System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
void Tao_OpenGl_DepthFunc_ctor(const std::shared_ptr<Tao_OpenGl_DepthFunc>& _delegate, void*, std::function<void(System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_DepthFunc_Invoke(const std::shared_ptr<Tao_OpenGl_DepthFunc>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct Tao_OpenGl_Hint {
	std::vector< std::function<void(System_Int32,System_Int32)> > _functions;
void Register(std::function<void(System_Int32,System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0, System_Int32 arg1) {
for(auto it : _functions)
	(it)(arg0, arg1);
}
}; //end of class delegate
void Tao_OpenGl_Hint_ctor(const std::shared_ptr<Tao_OpenGl_Hint>& _delegate, void*, std::function<void(System_Int32,System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Hint_Invoke(const std::shared_ptr<Tao_OpenGl_Hint>& _delegate, System_Int32 arg0, System_Int32 arg1){
  _delegate->Invoke(arg0, arg1);
}
struct Tao_OpenGl_Viewport {
	std::vector< std::function<void(System_Int32,System_Int32,System_Int32,System_Int32)> > _functions;
void Register(std::function<void(System_Int32,System_Int32,System_Int32,System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0, System_Int32 arg1, System_Int32 arg2, System_Int32 arg3) {
for(auto it : _functions)
	(it)(arg0, arg1, arg2, arg3);
}
}; //end of class delegate
void Tao_OpenGl_Viewport_ctor(const std::shared_ptr<Tao_OpenGl_Viewport>& _delegate, void*, std::function<void(System_Int32,System_Int32,System_Int32,System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Viewport_Invoke(const std::shared_ptr<Tao_OpenGl_Viewport>& _delegate, System_Int32 arg0, System_Int32 arg1, System_Int32 arg2, System_Int32 arg3){
  _delegate->Invoke(arg0, arg1, arg2, arg3);
}
struct Tao_OpenGl_MatrixMode {
	std::vector< std::function<void(System_Int32)> > _functions;
void Register(std::function<void(System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
void Tao_OpenGl_MatrixMode_ctor(const std::shared_ptr<Tao_OpenGl_MatrixMode>& _delegate, void*, std::function<void(System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_MatrixMode_Invoke(const std::shared_ptr<Tao_OpenGl_MatrixMode>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct Tao_OpenGl_LoadIdentity {
	std::vector< std::function<void()> > _functions;
void Register(std::function<void() > fn) {
_functions.push_back(fn);
}
void Invoke() {
for(auto it : _functions)
	(it)();
}
}; //end of class delegate
void Tao_OpenGl_LoadIdentity_ctor(const std::shared_ptr<Tao_OpenGl_LoadIdentity>& _delegate, void*, std::function<void()> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_LoadIdentity_Invoke(const std::shared_ptr<Tao_OpenGl_LoadIdentity>& _delegate, ){
  _delegate->Invoke();
}
struct Tao_OpenGl_Clear {
	std::vector< std::function<void(System_Int32)> > _functions;
void Register(std::function<void(System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
void Tao_OpenGl_Clear_ctor(const std::shared_ptr<Tao_OpenGl_Clear>& _delegate, void*, std::function<void(System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Clear_Invoke(const std::shared_ptr<Tao_OpenGl_Clear>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct Tao_OpenGl_Translatef {
	std::vector< std::function<void(System_Single,System_Single,System_Single)> > _functions;
void Register(std::function<void(System_Single,System_Single,System_Single) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Single arg0, System_Single arg1, System_Single arg2) {
for(auto it : _functions)
	(it)(arg0, arg1, arg2);
}
}; //end of class delegate
void Tao_OpenGl_Translatef_ctor(const std::shared_ptr<Tao_OpenGl_Translatef>& _delegate, void*, std::function<void(System_Single,System_Single,System_Single)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Translatef_Invoke(const std::shared_ptr<Tao_OpenGl_Translatef>& _delegate, System_Single arg0, System_Single arg1, System_Single arg2){
  _delegate->Invoke(arg0, arg1, arg2);
}
struct Tao_OpenGl_Rotatef {
	std::vector< std::function<void(System_Single,System_Single,System_Single,System_Single)> > _functions;
void Register(std::function<void(System_Single,System_Single,System_Single,System_Single) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Single arg0, System_Single arg1, System_Single arg2, System_Single arg3) {
for(auto it : _functions)
	(it)(arg0, arg1, arg2, arg3);
}
}; //end of class delegate
void Tao_OpenGl_Rotatef_ctor(const std::shared_ptr<Tao_OpenGl_Rotatef>& _delegate, void*, std::function<void(System_Single,System_Single,System_Single,System_Single)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Rotatef_Invoke(const std::shared_ptr<Tao_OpenGl_Rotatef>& _delegate, System_Single arg0, System_Single arg1, System_Single arg2, System_Single arg3){
  _delegate->Invoke(arg0, arg1, arg2, arg3);
}
struct Tao_OpenGl_Begin {
	std::vector< std::function<void(System_Int32)> > _functions;
void Register(std::function<void(System_Int32) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
void Tao_OpenGl_Begin_ctor(const std::shared_ptr<Tao_OpenGl_Begin>& _delegate, void*, std::function<void(System_Int32)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Begin_Invoke(const std::shared_ptr<Tao_OpenGl_Begin>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct Tao_OpenGl_Color3f {
	std::vector< std::function<void(System_Single,System_Single,System_Single)> > _functions;
void Register(std::function<void(System_Single,System_Single,System_Single) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Single arg0, System_Single arg1, System_Single arg2) {
for(auto it : _functions)
	(it)(arg0, arg1, arg2);
}
}; //end of class delegate
void Tao_OpenGl_Color3f_ctor(const std::shared_ptr<Tao_OpenGl_Color3f>& _delegate, void*, std::function<void(System_Single,System_Single,System_Single)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Color3f_Invoke(const std::shared_ptr<Tao_OpenGl_Color3f>& _delegate, System_Single arg0, System_Single arg1, System_Single arg2){
  _delegate->Invoke(arg0, arg1, arg2);
}
struct Tao_OpenGl_Vertex3f {
	std::vector< std::function<void(System_Single,System_Single,System_Single)> > _functions;
void Register(std::function<void(System_Single,System_Single,System_Single) > fn) {
_functions.push_back(fn);
}
void Invoke(System_Single arg0, System_Single arg1, System_Single arg2) {
for(auto it : _functions)
	(it)(arg0, arg1, arg2);
}
}; //end of class delegate
void Tao_OpenGl_Vertex3f_ctor(const std::shared_ptr<Tao_OpenGl_Vertex3f>& _delegate, void*, std::function<void(System_Single,System_Single,System_Single)> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_Vertex3f_Invoke(const std::shared_ptr<Tao_OpenGl_Vertex3f>& _delegate, System_Single arg0, System_Single arg1, System_Single arg2){
  _delegate->Invoke(arg0, arg1, arg2);
}
struct Tao_OpenGl_End {
	std::vector< std::function<void()> > _functions;
void Register(std::function<void() > fn) {
_functions.push_back(fn);
}
void Invoke() {
for(auto it : _functions)
	(it)();
}
}; //end of class delegate
void Tao_OpenGl_End_ctor(const std::shared_ptr<Tao_OpenGl_End>& _delegate, void*, std::function<void()> fn){
  _delegate->Register(fn);
}
void Tao_OpenGl_End_Invoke(const std::shared_ptr<Tao_OpenGl_End>& _delegate, ){
  _delegate->Invoke();
}

System_Void Game_App_Main();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

// --- End of definition of virtual method tables ---

///--- PInvoke code --- 
///---Begin closure code --- 
System_Void Game_App_Main()

{

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
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

