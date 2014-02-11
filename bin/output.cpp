#include "sloth.h"
struct Tao_Sdl_SDL_keysym; 
struct Tao_Sdl_SDL_MouseMotionEvent; 
struct Tao_Sdl_SDL_JoyButtonEvent; 
struct Tao_Sdl_SDL_ActiveEvent; 
struct Tao_Sdl_SDL_JoyBallEvent; 
struct Tao_Sdl_SDL_JoyHatEvent; 
struct Tao_Sdl_SDL_ResizeEvent; 
struct Tao_Sdl_SDL_JoyAxisEvent; 
struct Tao_Sdl_SDL_MouseButtonEvent; 
struct Tao_Sdl_SDL_QuitEvent; 
struct Tao_Sdl_SDL_ExposeEvent; 
struct Tao_Sdl_SDL_SysWMEvent; 
struct Tao_Sdl_SDL_KeyboardEvent; 
struct Tao_Sdl_SDL_UserEvent; 
struct Tao_Sdl_SDL_Event; 
struct SimpleAdditions_CrGl; 
struct SimpleAdditions_CrSdl; 
struct SimpleAdditions_CrGlu; 
struct System_Math; 
struct System_Array; 
struct System_ValueType; 
struct System_IO_File; 
struct System_Console; 
struct System_Object; 
struct System_String; 
struct Game_GameClass; 
struct Game_App; 
struct Tao_Sdl_SDL_keysym {
 System_Byte scancode;
 System_Int32 sym;
 System_Int32 mod;
 System_Int16 unicode;
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
struct Tao_Sdl_SDL_JoyButtonEvent {
 System_Byte type;
 System_Byte which;
 System_Byte button;
 System_Byte state;
};
struct Tao_Sdl_SDL_ActiveEvent {
 System_Byte type;
 System_Byte gain;
 System_Byte state;
};
struct Tao_Sdl_SDL_JoyBallEvent {
 System_Byte type;
 System_Byte which;
 System_Byte ball;
 System_Int16 xrel;
 System_Int16 yrel;
};
struct Tao_Sdl_SDL_JoyHatEvent {
 System_Byte type;
 System_Byte which;
 System_Byte hat;
 System_Byte val;
};
struct Tao_Sdl_SDL_ResizeEvent {
 System_Byte type;
 System_Int32 w;
 System_Int32 h;
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
struct Tao_Sdl_SDL_QuitEvent {
 System_Byte type;
};
struct Tao_Sdl_SDL_ExposeEvent {
 System_Byte type;
};
struct Tao_Sdl_SDL_SysWMEvent {
 System_Byte type;
 System_IntPtr msg;
};
struct Tao_Sdl_SDL_KeyboardEvent {
 System_Byte type;
 System_Byte which;
 System_Byte state;
 Tao_Sdl_SDL_keysym keysym;
};
struct Tao_Sdl_SDL_UserEvent {
 System_Byte type;
 System_Int32 code;
 System_IntPtr data1;
 System_IntPtr data2;
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
struct SimpleAdditions_CrGl {
};
struct SimpleAdditions_CrSdl {
};
struct SimpleAdditions_CrGlu {
};
struct System_Math {
};
struct System_Array {
};
struct System_ValueType {
};
struct System_IO_File {
};
struct System_Console {
};
struct System_Object {
};
struct System_String {
 std::shared_ptr< Array < System_Char > > Text;
};
struct Game_GameClass {
 System_Boolean fin;
 Tao_Sdl_SDL_Event e;
 System_Single xrot;
 System_Single yrot;
};
struct Game_App {
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

System_Void Game_App_Main();

System_Void Game_App_ctor(Game_App * _this);

System_Void Game_App_setSDLVideo(Game_App * _this);

System_Boolean Game_App_InitGL();

System_Void Game_App_ReSizeGLScene(System_Int32 width, System_Int32 height);

System_Void Game_GameClass_ctor(Game_GameClass * _this);

System_Void Game_GameClass_pollEvents(Game_GameClass * _this);

System_Void Game_GameClass_set_finP(Game_GameClass * _this, System_Boolean  value);

System_Void Game_App_Tick();

System_Boolean Game_App_DrawGLScene();

System_Boolean Game_GameClass_get_finP(Game_GameClass * _this);

#include "runtime_base.hpp"
#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
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

typedef System_Int32 (__cdecl *dll_method_3_type)(std::shared_ptr<System_String> variable);
dll_method_3_type dll_method_3;
System_Int32 Tao_Sdl_Sdl_SDL_putenv(std::shared_ptr<System_String> variable)
{
return dll_method_3(variable);
}

typedef System_Int32 (__cdecl *dll_method_4_type)(System_Int32 attr, System_Int32 val);
dll_method_4_type dll_method_4;
System_Int32 Tao_Sdl_Sdl_SDL_GL_SetAttribute(System_Int32 attr, System_Int32 val)
{
return dll_method_4(attr, val);
}

typedef System_IntPtr (__cdecl *dll_method_5_type)(System_Int32 width, System_Int32 height, System_Int32 bpp, System_Int32 flags);
dll_method_5_type dll_method_5;
System_IntPtr Tao_Sdl_Sdl_SDL_SetVideoMode(System_Int32 width, System_Int32 height, System_Int32 bpp, System_Int32 flags)
{
return dll_method_5(width, height, bpp, flags);
}

typedef void (__stdcall *dll_method_6_type)(System_Int32 mode);
dll_method_6_type dll_method_6;
System_Void Tao_OpenGl_Gl_glShadeModel(System_Int32 mode)
{
dll_method_6(mode);
}

typedef void (__stdcall *dll_method_7_type)(System_Single red, System_Single green, System_Single blue, System_Single alpha);
dll_method_7_type dll_method_7;
System_Void Tao_OpenGl_Gl_glClearColor(System_Single red, System_Single green, System_Single blue, System_Single alpha)
{
dll_method_7(red, green, blue, alpha);
}

typedef void (__stdcall *dll_method_8_type)(System_Double depth);
dll_method_8_type dll_method_8;
System_Void Tao_OpenGl_Gl_glClearDepth(System_Double depth)
{
dll_method_8(depth);
}

typedef void (__stdcall *dll_method_9_type)(System_Int32 cap);
dll_method_9_type dll_method_9;
System_Void Tao_OpenGl_Gl_glEnable(System_Int32 cap)
{
dll_method_9(cap);
}

typedef void (__stdcall *dll_method_10_type)(System_Int32 func);
dll_method_10_type dll_method_10;
System_Void Tao_OpenGl_Gl_glDepthFunc(System_Int32 func)
{
dll_method_10(func);
}

typedef void (__stdcall *dll_method_11_type)(System_Int32 target, System_Int32 mode);
dll_method_11_type dll_method_11;
System_Void Tao_OpenGl_Gl_glHint(System_Int32 target, System_Int32 mode)
{
dll_method_11(target, mode);
}

typedef void (__stdcall *dll_method_12_type)(System_Int32 x, System_Int32 y, System_Int32 width, System_Int32 height);
dll_method_12_type dll_method_12;
System_Void Tao_OpenGl_Gl_glViewport(System_Int32 x, System_Int32 y, System_Int32 width, System_Int32 height)
{
dll_method_12(x, y, width, height);
}

typedef void (__stdcall *dll_method_13_type)(System_Int32 mode);
dll_method_13_type dll_method_13;
System_Void Tao_OpenGl_Gl_glMatrixMode(System_Int32 mode)
{
dll_method_13(mode);
}

typedef void (__stdcall *dll_method_14_type)();
dll_method_14_type dll_method_14;
System_Void Tao_OpenGl_Gl_glLoadIdentity()
{
dll_method_14();
}

typedef void (__stdcall *dll_method_15_type)(System_Double fovY, System_Double aspectRatio, System_Double zNear, System_Double zFar);
dll_method_15_type dll_method_15;
System_Void Tao_OpenGl_Glu_gluPerspective(System_Double fovY, System_Double aspectRatio, System_Double zNear, System_Double zFar)
{
dll_method_15(fovY, aspectRatio, zNear, zFar);
}

typedef System_Int32 (__cdecl *dll_method_16_type)(System_Int32 toggle);
dll_method_16_type dll_method_16;
System_Int32 Tao_Sdl_Sdl_SDL_ShowCursor(System_Int32 toggle)
{
return dll_method_16(toggle);
}

typedef System_Int32 (__cdecl *dll_method_17_type)(System_Int32 mode);
dll_method_17_type dll_method_17;
System_Int32 Tao_Sdl_Sdl_SDL_WM_GrabInput(System_Int32 mode)
{
return dll_method_17(mode);
}

typedef System_Int32 (__cdecl *dll_method_18_type)(Tao_Sdl_SDL_Event*  sdlEvent);
dll_method_18_type dll_method_18;
System_Int32 Tao_Sdl_Sdl_SDL_PollEvent(Tao_Sdl_SDL_Event*  sdlEvent)
{
return dll_method_18(sdlEvent);
}

typedef void (__stdcall *dll_method_19_type)(System_Int32 mask);
dll_method_19_type dll_method_19;
System_Void Tao_OpenGl_Gl_glClear(System_Int32 mask)
{
dll_method_19(mask);
}

typedef void (__stdcall *dll_method_20_type)(System_Single x, System_Single y, System_Single z);
dll_method_20_type dll_method_20;
System_Void Tao_OpenGl_Gl_glTranslatef(System_Single x, System_Single y, System_Single z)
{
dll_method_20(x, y, z);
}

typedef void (__stdcall *dll_method_21_type)(System_Single angle, System_Single x, System_Single y, System_Single z);
dll_method_21_type dll_method_21;
System_Void Tao_OpenGl_Gl_glRotatef(System_Single angle, System_Single x, System_Single y, System_Single z)
{
dll_method_21(angle, x, y, z);
}

typedef void (__stdcall *dll_method_22_type)(System_Int32 mode);
dll_method_22_type dll_method_22;
System_Void Tao_OpenGl_Gl_glBegin(System_Int32 mode)
{
dll_method_22(mode);
}

typedef void (__stdcall *dll_method_23_type)(System_Single red, System_Single green, System_Single blue);
dll_method_23_type dll_method_23;
System_Void Tao_OpenGl_Gl_glColor3f(System_Single red, System_Single green, System_Single blue)
{
dll_method_23(red, green, blue);
}

typedef void (__stdcall *dll_method_24_type)(System_Single x, System_Single y, System_Single z);
dll_method_24_type dll_method_24;
System_Void Tao_OpenGl_Gl_glVertex3f(System_Single x, System_Single y, System_Single z)
{
dll_method_24(x, y, z);
}

typedef void (__stdcall *dll_method_25_type)();
dll_method_25_type dll_method_25;
System_Void Tao_OpenGl_Gl_glEnd()
{
dll_method_25();
}

typedef void (__cdecl *dll_method_26_type)();
dll_method_26_type dll_method_26;
System_Void Tao_Sdl_Sdl_SDL_GL_SwapBuffers()
{
dll_method_26();
}

typedef void (__cdecl *dll_method_27_type)(System_Int32 ms);
dll_method_27_type dll_method_27;
System_Void Tao_Sdl_Sdl_SDL_Delay(System_Int32 ms)
{
dll_method_27(ms);
}

///---Begin closure code --- 
System_Void Game_App_Main()
{

Game_App  vreg_1;
Game_App_ctor(&vreg_1);
return;
}


System_Void Game_App_ctor(Game_App * _this)
{
System_Boolean local_0;
System_Int32 vreg_11;
System_Int32 vreg_13;
System_Boolean vreg_16;
std::shared_ptr<Game_GameClass> vreg_20;
Game_GameClass * vreg_23;
Game_GameClass * vreg_26;
System_Boolean vreg_27;
System_Boolean vreg_30;

_this->w = 1024;
_this->h = 768;
_this->bpp = 16;
_this->flags = 7;
vreg_11 = Tao_Sdl_Sdl_SDL_Init(32);
vreg_13 = (vreg_11 < 0)?1:0;
vreg_16 = (vreg_13 == 0)?1:0;
if(vreg_16) goto label_82;
System_Console_WriteLine(_str(0));
Tao_Sdl_Sdl_SDL_Quit();
goto label_155;
label_82:
Game_App_setSDLVideo(_this);
vreg_20 = std::make_shared<Game_GameClass >();
Game_GameClass_ctor(vreg_20.get());
_this->game = vreg_20;
vreg_26 = _this->game.get();
vreg_23 = _this->game.get();
goto label_129;
label_102:
Game_GameClass_pollEvents(vreg_23);
Game_App_Tick();
Tao_Sdl_Sdl_SDL_Delay(1);
label_129:
vreg_27 = Game_GameClass_get_finP(vreg_26);
vreg_30 = (vreg_27 == 0)?1:0;
if(vreg_30) goto label_102;
Tao_Sdl_Sdl_SDL_Quit();
label_155:
return;
}


System_Void Game_App_setSDLVideo(Game_App * _this)
{
System_IntPtr local_0;
System_Int32 vreg_28;
System_Int32 vreg_30;
System_Int32 vreg_32;
System_Int32 vreg_34;
System_IntPtr vreg_35;
System_Boolean vreg_38;
System_Boolean vreg_41;
System_Int32 vreg_45;
System_Int32 vreg_47;

Tao_Sdl_Sdl_SDL_putenv(_str(1));
Tao_Sdl_Sdl_SDL_GL_SetAttribute(16, 1);
Tao_Sdl_Sdl_SDL_GL_SetAttribute(0, 8);
Tao_Sdl_Sdl_SDL_GL_SetAttribute(1, 8);
Tao_Sdl_Sdl_SDL_GL_SetAttribute(2, 8);
Tao_Sdl_Sdl_SDL_GL_SetAttribute(6, 16);
Tao_Sdl_Sdl_SDL_GL_SetAttribute(13, 1);
Tao_Sdl_Sdl_SDL_GL_SetAttribute(14, 2);
Tao_Sdl_Sdl_SDL_GL_SetAttribute(5, 1);
vreg_28 = _this->w;
vreg_30 = _this->h;
vreg_32 = _this->bpp;
vreg_34 = _this->flags;
vreg_35 = Tao_Sdl_Sdl_SDL_SetVideoMode(vreg_28, vreg_30, vreg_32, vreg_34);
vreg_38 = System_IntPtr_op_Equality((System_IntPtr)vreg_35, 0);
vreg_41 = (vreg_38 == 0)?1:0;
if(vreg_41) goto label_147;
System_Console_WriteLine(_str(2));
Tao_Sdl_Sdl_SDL_Quit();
label_147:
Game_App_InitGL();
vreg_45 = _this->w;
vreg_47 = _this->h;
Game_App_ReSizeGLScene(vreg_45, vreg_47);
return;
}


System_Boolean Game_App_InitGL()
{

Tao_OpenGl_Gl_glShadeModel(7425);
Tao_OpenGl_Gl_glClearColor(0, 0, 0, 0.5);
Tao_OpenGl_Gl_glClearDepth(1);
Tao_OpenGl_Gl_glEnable(2929);
Tao_OpenGl_Gl_glDepthFunc(515);
Tao_OpenGl_Gl_glHint(3152, 4354);
return 1;
}


System_Void Game_App_ReSizeGLScene(System_Int32 width, System_Int32 height)
{
System_Boolean local_0;
System_Int32 vreg_3;
System_Boolean vreg_6;
System_Double vreg_15;
System_Double vreg_17;
System_Double vreg_18;

vreg_3 = (height == 0)?1:0;
vreg_6 = (vreg_3 == 0)?1:0;
if(vreg_6) goto label_17;
height = 1;
label_17:
Tao_OpenGl_Gl_glViewport(0, 0, width, height);
Tao_OpenGl_Gl_glMatrixMode(5889);
Tao_OpenGl_Gl_glLoadIdentity();
vreg_15 = (double)width;
vreg_17 = (double)height;
vreg_18 = vreg_15/vreg_17;
Tao_OpenGl_Glu_gluPerspective(45, vreg_18, 0.1, 100);
Tao_OpenGl_Gl_glMatrixMode(5888);
Tao_OpenGl_Gl_glLoadIdentity();
return;
}


System_Void Game_GameClass_ctor(Game_GameClass * _this)
{
Tao_Sdl_SDL_Event*  vreg_13;

_this->fin = 0;
_this->xrot = 0;
_this->yrot = 0;
Tao_Sdl_Sdl_SDL_ShowCursor(0);
Tao_Sdl_Sdl_SDL_WM_GrabInput(1);
vreg_13 = &_this->e;
return;
}


System_Void Game_GameClass_pollEvents(Game_GameClass * _this)
{
System_Byte local_0;
Tao_Sdl_SDL_Event*  vreg_2;
System_Byte vreg_3;
System_Byte vreg_6;
System_Byte vreg_7;
Tao_Sdl_SDL_Event*  vreg_12;
Tao_Sdl_SDL_KeyboardEvent*  vreg_13;
Tao_Sdl_SDL_keysym*  vreg_14;
System_Int32 vreg_15;
System_Int32 vreg_17;
System_Boolean vreg_20;
Tao_Sdl_SDL_Event*  vreg_24;
Tao_Sdl_SDL_KeyboardEvent*  vreg_25;
Tao_Sdl_SDL_keysym*  vreg_26;
System_Int32 vreg_27;
System_Int32 vreg_29;
System_Boolean vreg_32;
Tao_Sdl_SDL_Event*  vreg_35;
Tao_Sdl_SDL_KeyboardEvent*  vreg_36;
Tao_Sdl_SDL_keysym*  vreg_37;
System_Int32 vreg_38;
System_Int32 vreg_40;
System_Boolean vreg_43;
Tao_Sdl_SDL_Event*  vreg_46;
Tao_Sdl_SDL_KeyboardEvent*  vreg_47;
Tao_Sdl_SDL_keysym*  vreg_48;
System_Int32 vreg_49;
System_Int32 vreg_51;
System_Boolean vreg_54;
Tao_Sdl_SDL_Event*  vreg_57;
Tao_Sdl_SDL_KeyboardEvent*  vreg_58;
Tao_Sdl_SDL_keysym*  vreg_59;
System_Int32 vreg_60;
System_Int32 vreg_62;
System_Boolean vreg_65;
Tao_Sdl_SDL_Event*  vreg_68;
Tao_Sdl_SDL_MouseButtonEvent*  vreg_69;
System_Byte vreg_70;
System_Byte vreg_72;
System_Boolean vreg_75;
Tao_Sdl_SDL_Event*  vreg_78;
Tao_Sdl_SDL_MouseButtonEvent*  vreg_79;
System_Byte vreg_80;
System_Byte vreg_82;
System_Boolean vreg_85;
Tao_Sdl_SDL_Event*  vreg_89;
System_Int32 vreg_90;
System_Boolean vreg_93;

goto label_369;
label_6:
vreg_2 = &_this->e;
vreg_3 = vreg_2->type;
local_0 = vreg_3;
vreg_6 = vreg_3-2;
if(vreg_6==0) goto label_65;
if(vreg_6==1) goto label_368;
if(vreg_6==2) goto label_355;
if(vreg_6==3) goto label_279;
vreg_7 = local_0;
if(12==vreg_7) goto label_53;
goto label_368;
label_53:
_this->fin = 1;
goto label_368;
label_65:
vreg_12 = &_this->e;
vreg_13 = &vreg_12->key;
vreg_14 = &vreg_13->keysym;
vreg_15 = vreg_14->sym;
vreg_17 = (vreg_15 == 27)?1:0;
vreg_20 = (vreg_17 == 0)?1:0;
if(vreg_20) goto label_105;
Game_GameClass_set_finP(_this, 1);
label_105:
vreg_24 = &_this->e;
vreg_25 = &vreg_24->key;
vreg_26 = &vreg_25->keysym;
vreg_27 = vreg_26->sym;
vreg_29 = (vreg_27 == 101)?1:0;
vreg_32 = (vreg_29 == 0)?1:0;
if(vreg_32) goto label_148;
System_Console_WriteLine(_str(3));
label_148:
vreg_35 = &_this->e;
vreg_36 = &vreg_35->key;
vreg_37 = &vreg_36->keysym;
vreg_38 = vreg_37->sym;
vreg_40 = (vreg_38 == 102)?1:0;
vreg_43 = (vreg_40 == 0)?1:0;
if(vreg_43) goto label_191;
System_Console_WriteLine(_str(4));
label_191:
vreg_46 = &_this->e;
vreg_47 = &vreg_46->key;
vreg_48 = &vreg_47->keysym;
vreg_49 = vreg_48->sym;
vreg_51 = (vreg_49 == 97)?1:0;
vreg_54 = (vreg_51 == 0)?1:0;
if(vreg_54) goto label_234;
System_Console_WriteLine(_str(5));
label_234:
vreg_57 = &_this->e;
vreg_58 = &vreg_57->key;
vreg_59 = &vreg_58->keysym;
vreg_60 = vreg_59->sym;
vreg_62 = (vreg_60 == 32)?1:0;
vreg_65 = (vreg_62 == 0)?1:0;
if(vreg_65) goto label_277;
System_Console_WriteLine(_str(6));
label_277:
goto label_368;
label_279:
vreg_68 = &_this->e;
vreg_69 = &vreg_68->button;
vreg_70 = vreg_69->button;
vreg_72 = (vreg_70 == 1)?1:0;
vreg_75 = (vreg_72 == 0)?1:0;
if(vreg_75) goto label_316;
System_Console_WriteLine(_str(7));
label_316:
vreg_78 = &_this->e;
vreg_79 = &vreg_78->button;
vreg_80 = vreg_79->button;
vreg_82 = (vreg_80 == 3)?1:0;
vreg_85 = (vreg_82 == 0)?1:0;
if(vreg_85) goto label_353;
System_Console_WriteLine(_str(8));
label_353:
goto label_368;
label_355:
System_Console_WriteLine(_str(9));
label_368:
label_369:
vreg_89 = &_this->e;
vreg_90 = Tao_Sdl_Sdl_SDL_PollEvent(vreg_89);
vreg_93 = (vreg_90 == 1)?1:0;
if(vreg_93) goto label_6;
return;
}


System_Void Game_GameClass_set_finP(Game_GameClass * _this, System_Boolean  value)
{

_this->fin = value;
return;
}


System_Void Game_App_Tick()
{

Game_App_DrawGLScene();
Tao_Sdl_Sdl_SDL_GL_SwapBuffers();
return;
}


System_Boolean Game_App_DrawGLScene()
{
System_Single vreg_9;
System_Single vreg_89;
System_Single vreg_187;
System_Single vreg_189;
System_Single vreg_190;
System_Single vreg_192;

Tao_OpenGl_Gl_glClearColor(1, 1, 0, 0);
Tao_OpenGl_Gl_glClear(16640);
Tao_OpenGl_Gl_glLoadIdentity();
Tao_OpenGl_Gl_glTranslatef(-1.5, 0, -6);
vreg_9 = Game_App::rtri;
Tao_OpenGl_Gl_glRotatef(vreg_9, 0, 1, 0);
Tao_OpenGl_Gl_glBegin(4);
Tao_OpenGl_Gl_glColor3f(1, 0, 0);
Tao_OpenGl_Gl_glVertex3f(0, 1, 0);
Tao_OpenGl_Gl_glColor3f(0, 1, 0);
Tao_OpenGl_Gl_glVertex3f(-1, -1, 1);
Tao_OpenGl_Gl_glColor3f(0, 0, 1);
Tao_OpenGl_Gl_glVertex3f(1, -1, 1);
Tao_OpenGl_Gl_glColor3f(1, 0, 0);
Tao_OpenGl_Gl_glVertex3f(0, 1, 0);
Tao_OpenGl_Gl_glColor3f(0, 0, 1);
Tao_OpenGl_Gl_glVertex3f(1, -1, 1);
Tao_OpenGl_Gl_glColor3f(0, 1, 0);
Tao_OpenGl_Gl_glVertex3f(1, -1, -1);
Tao_OpenGl_Gl_glColor3f(1, 0, 0);
Tao_OpenGl_Gl_glVertex3f(0, 1, 0);
Tao_OpenGl_Gl_glColor3f(0, 1, 0);
Tao_OpenGl_Gl_glVertex3f(1, -1, -1);
Tao_OpenGl_Gl_glColor3f(0, 0, 1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, -1);
Tao_OpenGl_Gl_glColor3f(1, 0, 0);
Tao_OpenGl_Gl_glVertex3f(0, 1, 0);
Tao_OpenGl_Gl_glColor3f(0, 0, 1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, -1);
Tao_OpenGl_Gl_glColor3f(0, 1, 0);
Tao_OpenGl_Gl_glVertex3f(-1, -1, 1);
Tao_OpenGl_Gl_glEnd();
Tao_OpenGl_Gl_glLoadIdentity();
Tao_OpenGl_Gl_glTranslatef(1.5, 0, -7);
vreg_89 = Game_App::rquad;
Tao_OpenGl_Gl_glRotatef(vreg_89, 1, 1, 1);
Tao_OpenGl_Gl_glColor3f(0.5, 0.5, 1);
Tao_OpenGl_Gl_glBegin(7);
Tao_OpenGl_Gl_glColor3f(0, 1, 0);
Tao_OpenGl_Gl_glVertex3f(1, 1, -1);
Tao_OpenGl_Gl_glVertex3f(-1, 1, -1);
Tao_OpenGl_Gl_glVertex3f(-1, 1, 1);
Tao_OpenGl_Gl_glVertex3f(1, 1, 1);
Tao_OpenGl_Gl_glColor3f(1, 0.5, 0);
Tao_OpenGl_Gl_glVertex3f(1, -1, 1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, 1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, -1);
Tao_OpenGl_Gl_glVertex3f(1, -1, -1);
Tao_OpenGl_Gl_glColor3f(1, 0, 0);
Tao_OpenGl_Gl_glVertex3f(1, 1, 1);
Tao_OpenGl_Gl_glVertex3f(-1, 1, 1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, 1);
Tao_OpenGl_Gl_glVertex3f(1, -1, 1);
Tao_OpenGl_Gl_glColor3f(1, 1, 0);
Tao_OpenGl_Gl_glVertex3f(1, -1, -1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, -1);
Tao_OpenGl_Gl_glVertex3f(-1, 1, -1);
Tao_OpenGl_Gl_glVertex3f(1, 1, -1);
Tao_OpenGl_Gl_glColor3f(0, 0, 1);
Tao_OpenGl_Gl_glVertex3f(-1, 1, 1);
Tao_OpenGl_Gl_glVertex3f(-1, 1, -1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, -1);
Tao_OpenGl_Gl_glVertex3f(-1, -1, 1);
Tao_OpenGl_Gl_glColor3f(1, 0, 1);
Tao_OpenGl_Gl_glVertex3f(1, 1, -1);
Tao_OpenGl_Gl_glVertex3f(1, 1, 1);
Tao_OpenGl_Gl_glVertex3f(1, -1, 1);
Tao_OpenGl_Gl_glVertex3f(1, -1, -1);
Tao_OpenGl_Gl_glEnd();
vreg_187 = Game_App::rtri;
vreg_189 = vreg_187+0.200000002980232;
Game_App::rtri = vreg_189;
vreg_190 = Game_App::rquad;
vreg_192 = vreg_190-0.150000005960464;
Game_App::rquad = vreg_192;
return 1;
}


System_Boolean Game_GameClass_get_finP(Game_GameClass * _this)
{
System_Boolean local_0;
System_Boolean vreg_2;

vreg_2 = _this->fin;
return vreg_2;
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
dll_method_3 = (dll_method_3_type)LoadNativeMethod(lib_0, "SDL_putenv");
dll_method_4 = (dll_method_4_type)LoadNativeMethod(lib_0, "SDL_GL_SetAttribute");
dll_method_5 = (dll_method_5_type)LoadNativeMethod(lib_0, "SDL_SetVideoMode");
dll_method_16 = (dll_method_16_type)LoadNativeMethod(lib_0, "SDL_ShowCursor");
dll_method_17 = (dll_method_17_type)LoadNativeMethod(lib_0, "SDL_WM_GrabInput");
dll_method_18 = (dll_method_18_type)LoadNativeMethod(lib_0, "SDL_PollEvent");
dll_method_26 = (dll_method_26_type)LoadNativeMethod(lib_0, "SDL_GL_SwapBuffers");
dll_method_27 = (dll_method_27_type)LoadNativeMethod(lib_0, "SDL_Delay");
auto lib_1 = LoadNativeLibrary(L"opengl32.dll");
dll_method_6 = (dll_method_6_type)LoadNativeMethod(lib_1, "glShadeModel");
dll_method_7 = (dll_method_7_type)LoadNativeMethod(lib_1, "glClearColor");
dll_method_8 = (dll_method_8_type)LoadNativeMethod(lib_1, "glClearDepth");
dll_method_9 = (dll_method_9_type)LoadNativeMethod(lib_1, "glEnable");
dll_method_10 = (dll_method_10_type)LoadNativeMethod(lib_1, "glDepthFunc");
dll_method_11 = (dll_method_11_type)LoadNativeMethod(lib_1, "glHint");
dll_method_12 = (dll_method_12_type)LoadNativeMethod(lib_1, "glViewport");
dll_method_13 = (dll_method_13_type)LoadNativeMethod(lib_1, "glMatrixMode");
dll_method_14 = (dll_method_14_type)LoadNativeMethod(lib_1, "glLoadIdentity");
dll_method_19 = (dll_method_19_type)LoadNativeMethod(lib_1, "glClear");
dll_method_20 = (dll_method_20_type)LoadNativeMethod(lib_1, "glTranslatef");
dll_method_21 = (dll_method_21_type)LoadNativeMethod(lib_1, "glRotatef");
dll_method_22 = (dll_method_22_type)LoadNativeMethod(lib_1, "glBegin");
dll_method_23 = (dll_method_23_type)LoadNativeMethod(lib_1, "glColor3f");
dll_method_24 = (dll_method_24_type)LoadNativeMethod(lib_1, "glVertex3f");
dll_method_25 = (dll_method_25_type)LoadNativeMethod(lib_1, "glEnd");
auto lib_2 = LoadNativeLibrary(L"glu32.dll");
dll_method_15 = (dll_method_15_type)LoadNativeMethod(lib_2, "gluPerspective");
}

void RuntimeHelpersBuildConstantTable() {
}

void buildStringTable() {
_AddJumpAndLength(0, 22);
_AddJumpAndLength(23, 25);
_AddJumpAndLength(49, 29);
_AddJumpAndLength(79, 11);
_AddJumpAndLength(91, 11);
_AddJumpAndLength(103, 13);
_AddJumpAndLength(117, 14);
_AddJumpAndLength(132, 23);
_AddJumpAndLength(156, 15);
_AddJumpAndLength(172, 12);
} // buildStringTable
const wchar_t _stringTable[185] = {
69, 114, 114, 111, 114, 32, 105, 110, 105, 116, 105, 97, 108, 105, 122, 105, 110, 103, 32, 83, 68, 76, 0 /* "Error initializing SDL" */, 
83, 68, 76, 95, 86, 73, 68, 69, 79, 95, 67, 69, 78, 84, 69, 82, 69, 68, 61, 99, 101, 110, 116, 101, 114, 0 /* "SDL_VIDEO_CENTERED=center" */, 
69, 114, 114, 111, 114, 32, 113, 115, 101, 116, 116, 105, 110, 103, 32, 116, 104, 101, 32, 118, 105, 100, 101, 111, 32, 109, 111, 100, 101, 0 /* "Error qsetting the video mode" */, 
77, 111, 118, 105, 110, 103, 32, 102, 111, 114, 100, 0 /* "Moving ford" */, 
77, 111, 118, 105, 110, 103, 32, 98, 97, 99, 107, 0 /* "Moving back" */, 
83, 116, 114, 97, 102, 105, 110, 103, 32, 108, 101, 102, 116, 0 /* "Strafing left" */, 
83, 116, 114, 97, 102, 105, 110, 103, 32, 114, 105, 103, 104, 116, 0 /* "Strafing right" */, 
76, 101, 116, 32, 116, 104, 101, 32, 115, 104, 111, 111, 115, 116, 105, 110, 103, 32, 98, 101, 103, 105, 110, 0 /* "Let the shoosting begin" */, 
85, 115, 105, 110, 103, 32, 97, 110, 32, 111, 98, 106, 101, 99, 116, 0 /* "Using an object" */, 
77, 111, 117, 115, 101, 32, 109, 111, 116, 105, 111, 110, 0 /* "Mouse motion" */
}; // _stringTable 

