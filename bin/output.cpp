#include "sloth.h"
#include <functional>
struct System_Object;
struct System_Console;
struct System_MulticastDelegate;
struct System_Action;
struct System_Delegate;
struct System_ValueType;
struct _NBody;
struct System_String;
struct System_Object {
	int _typeId;
};
struct System_Console : public System_Object {
};
struct System_MulticastDelegate : public System_Delegate {
};
struct System_Delegate : public System_Object {
};
struct System_ValueType : public System_Object {
};
struct _NBody : public System_Object {
	static std::shared_ptr<System_Action> AutoNamed_0;
};
/* static*/ std::shared_ptr<System_Action> _NBody::AutoNamed_0 = std::shared_ptr <std::shared_ptr<System_Action>>(0);
struct System_String : public System_Object {
	std::shared_ptr< Array < System_Char > > Text;
};
struct System_Action : System_Object {
	std::vector< std::function<System_Void()> > _functions;
	System_Void Register(std::function<System_Void() > fn) {
		_functions.push_back(fn);
	}
	System_Void Invoke() {
		for (auto it : _functions)
			(it)();
	}
}; //end of class delegate
System_Void System_Action_ctor(const std::shared_ptr<System_Action>& _delegate, System_Void*, std::function<System_Void()> fn){
	_delegate->Register(fn);
}
System_Void System_Action_Invoke(const std::shared_ptr<System_Action>& _delegate){
	_delegate->Invoke();
}

System_Void _NBody_Main();

System_Void System_Action_ctor(System_Action * _this, System_Object * object, System_IntPtr  method);

System_Void System_Action_Invoke(System_Action * _this);

System_Void _NBody__Main_b__0();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

typedef System_Void(*System_Action_InvokeVirtPtr)(const std::shared_ptr<System_Action> &_this);
System_Void System_Action_Invoke_vcall(const std::shared_ptr<System_Action> &_this);
System_Void System_Action_Invoke_vcall(const std::shared_ptr<System_Action> &_this){
	switch (_this->_typeId)
	{
	case 3:
		System_Action_Invoke(_this);
		return;
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
	std::shared_ptr<System_Action> local_0;
	std::shared_ptr<System_Action> vreg_1;
	System_Void(*vreg_2)(*);
	std::shared_ptr<System_Action> vreg_3;
	std::shared_ptr<System_Action> vreg_4;
	std::shared_ptr<System_Action> vreg_5;

	System_Console_WriteLine(_str(0));
	vreg_1 = _NBody::AutoNamed_0;
	if (vreg_1) goto label_22;
	vreg_2 = &(_NBody__Main_b__0);
	vreg_3 = std::make_shared<System_Action >();
	vreg_3->_typeId = 3;
	System_Action_ctor(vreg_3.get(), nullptr, vreg_2.get());
	vreg_4 = vreg_3;
	_NBody::AutoNamed_0 = vreg_4;
label_22:
	vreg_5 = _NBody::AutoNamed_0;
	local_0 = vreg_5;
	System_Action_Invoke(local_0.get());
	return;
}


System_Void System_Action_ctor(System_Action * _this, System_Object * object, System_IntPtr  method)

{

}


System_Void System_Action_Invoke(System_Action * _this)

{

}


System_Void _NBody__Main_b__0()

{

	System_Console_WriteLine(_str(1));
	return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
	auto argsAsList = System_getArgumentsAsList(argc, argv);
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

