#include "sloth.h"
#include <functional>
struct System_Object; 
struct _NBody; 
struct System_Console; 
struct _simple; 
struct System_ValueType; 
struct System_Delegate; 
struct System_MulticastDelegate; 
struct System_String; 
struct System_Object {
int _typeId;
};
struct _NBody : public System_Object {
};
struct System_Console : public System_Object {
};
struct System_ValueType : public System_Object {
};
struct System_Delegate : public System_Object {
};
struct System_MulticastDelegate : public System_Delegate {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct System_Action : System_Object {
	std::vector< std::function<System_Void()> > _functions;
System_Void Register(std::function<System_Void() > fn) {
_functions.push_back(fn);
}
System_Void Invoke() {
for(auto it : _functions)
	(it)();
}
}; //end of class delegate
System_Void System_Action_ctor(const std::shared_ptr<System_Action>& _delegate, System_Void*, std::function<System_Void()> fn){
  _delegate->Register(fn);
}
System_Void System_Action_Invoke(const std::shared_ptr<System_Action>& _delegate){
  _delegate->Invoke();
}
struct _simple : System_Object {
	std::vector< std::function<System_Void(System_Int32)> > _functions;
System_Void Register(std::function<System_Void(System_Int32) > fn) {
_functions.push_back(fn);
}
System_Void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
System_Void _simple_ctor(const std::shared_ptr<_simple>& _delegate, System_Void*, std::function<System_Void(System_Int32)> fn){
  _delegate->Register(fn);
}
System_Void _simple_Invoke(const std::shared_ptr<_simple>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct _simple : System_Object {
	std::vector< std::function<System_Void(System_Int32)> > _functions;
System_Void Register(std::function<System_Void(System_Int32) > fn) {
_functions.push_back(fn);
}
System_Void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
System_Void _simple_ctor(const std::shared_ptr<_simple>& _delegate, System_Void*, std::function<System_Void(System_Int32)> fn){
  _delegate->Register(fn);
}
System_Void _simple_Invoke(const std::shared_ptr<_simple>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct _simple : System_Object {
	std::vector< std::function<System_Void(System_Int32)> > _functions;
System_Void Register(std::function<System_Void(System_Int32) > fn) {
_functions.push_back(fn);
}
System_Void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
System_Void _simple_ctor(const std::shared_ptr<_simple>& _delegate, System_Void*, std::function<System_Void(System_Int32)> fn){
  _delegate->Register(fn);
}
System_Void _simple_Invoke(const std::shared_ptr<_simple>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct _simple : System_Object {
	std::vector< std::function<System_Void(System_Int32)> > _functions;
System_Void Register(std::function<System_Void(System_Int32) > fn) {
_functions.push_back(fn);
}
System_Void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
System_Void _simple_ctor(const std::shared_ptr<_simple>& _delegate, System_Void*, std::function<System_Void(System_Int32)> fn){
  _delegate->Register(fn);
}
System_Void _simple_Invoke(const std::shared_ptr<_simple>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct _simple : System_Object {
	std::vector< std::function<System_Void(System_Int32)> > _functions;
System_Void Register(std::function<System_Void(System_Int32) > fn) {
_functions.push_back(fn);
}
System_Void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
System_Void _simple_ctor(const std::shared_ptr<_simple>& _delegate, System_Void*, std::function<System_Void(System_Int32)> fn){
  _delegate->Register(fn);
}
System_Void _simple_Invoke(const std::shared_ptr<_simple>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct _simple : System_Object {
	std::vector< std::function<System_Void(System_Int32)> > _functions;
System_Void Register(std::function<System_Void(System_Int32) > fn) {
_functions.push_back(fn);
}
System_Void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
System_Void _simple_ctor(const std::shared_ptr<_simple>& _delegate, System_Void*, std::function<System_Void(System_Int32)> fn){
  _delegate->Register(fn);
}
System_Void _simple_Invoke(const std::shared_ptr<_simple>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}
struct _simple : System_Object {
	std::vector< std::function<System_Void(System_Int32)> > _functions;
System_Void Register(std::function<System_Void(System_Int32) > fn) {
_functions.push_back(fn);
}
System_Void Invoke(System_Int32 arg0) {
for(auto it : _functions)
	(it)(arg0);
}
}; //end of class delegate
System_Void _simple_ctor(const std::shared_ptr<_simple>& _delegate, System_Void*, std::function<System_Void(System_Int32)> fn){
  _delegate->Register(fn);
}
System_Void _simple_Invoke(const std::shared_ptr<_simple>& _delegate, System_Int32 arg0){
  _delegate->Invoke(arg0);
}

System_Void _NBody_Main();

System_Void _simple_ctor(_simple * _this, System_Object * object, System_IntPtr  method);

System_Int32 _simple_Invoke(_simple * _this, System_Int32  g);

System_Int32 _NBody_AddPrimes(System_Int32 len);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

typedef System_Int32 (*_simple_InvokeVirtPtr)(const std::shared_ptr<_simple> &_this);
System_Int32 _simple_Invoke_vcall(const std::shared_ptr<_simple> &_this);
System_Int32 _simple_Invoke_vcall(const std::shared_ptr<_simple> &_this){
switch(_this->_typeId)
{
case 3:
return _simple_Invoke(_this);
}
}
// --- End of definition of virtual method tables ---

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ 
	printf("%ls\n", value.get()->Text->Items); 
}

///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{
System_Int32 local_0;
_simple * local_1;
System_Void (*vreg_1)(System_Int32*);
_simple * vreg_3;
System_Int32 vreg_4;

System_Console_WriteLine(_str(0));
local_0 = 1000000;
vreg_1=&(_NBody_AddPrimes);
_simple  vreg_2;
vreg_2._typeId = 3;
_simple_ctor(&vreg_2, nullptr, vreg_1);
vreg_3 = &vreg_2;
local_1 = vreg_3;
vreg_4 = _simple_Invoke(local_1, local_0);
return;
}


System_Void _simple_ctor(_simple * _this, System_Object * object, System_IntPtr  method)

{

}


System_Int32 _simple_Invoke(_simple * _this, System_Int32  g)

{

}


System_Int32 _NBody_AddPrimes(System_Int32 len)

{
System_Int32 local_0;
System_Int32 local_1;
System_Boolean local_2;
System_Int32 local_3;
System_Int32 vreg_1;
System_Int32 vreg_2;
System_Int32 vreg_3;
System_Int32 vreg_4;
System_Int32 vreg_5;
System_Int32 vreg_6;

local_0 = 0;
local_1 = 2;
goto label_2F;
label_6:
vreg_1 = local_1%2;
if(!(vreg_1)) goto label_2B;
local_2 = 1;
local_3 = 2;
goto label_1E;
vreg_2 = local_1%local_3;
if(vreg_2) goto label_1A;
local_2 = 0;
goto label_24;
label_1A:
vreg_3 = local_3+1;
local_3 = vreg_3;
label_1E:
vreg_4 = local_3*local_3;
if(local_1<=vreg_4) goto label_11;
label_24:
if(!(local_2)) goto label_2B;
vreg_5 = local_0+1;
local_0 = vreg_5;
label_2B:
vreg_6 = local_1+1;
local_1 = vreg_6;
label_2F:
if(len<local_1) goto label_6;
return local_0;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
_NBody_Main();
return 0;
}
System_Void mapLibs() {
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
_AddJumpAndLength(0, 15);
_AddJumpAndLength(16, 17);
} // buildStringTable
const wchar_t _stringTable[34] = {
80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */, 
83, 105, 109, 112, 108, 101, 114, 32, 69, 120, 97, 109, 112, 108, 101, 58, 32, 0 /* "Simpler Example: " */
}; // _stringTable 

