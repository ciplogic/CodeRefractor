#include "sloth.h"
#include <functional>
struct System_Object;
struct _NBody;
struct ___c__DisplayClass1;
struct System_Console;
struct System_Action;
struct System_ValueType;
struct System_Delegate;
struct System_MulticastDelegate;
struct System_String;
struct System_Object {
	int _typeId;
};
struct _NBody : public System_Object {
};
struct ___c__DisplayClass1 : public System_Object {
	System_Int32 len;
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
	std::vector< std::function<System_Void(System_Void*)> > _functions;
	std::vector< System_Void *>  _objects;

	System_Void Register(std::function<System_Void(System_Void*) > fn, System_Void* object) {
		_functions.push_back(fn);
		_objects.push_back(object);
	}
	System_Void Invoke() {
		int count = 0;
		for (auto it : _functions)
		{
			auto obj = _objects[count];

			if (obj == NULL)
				it(NULL);
			else
				(it)(obj);
			count++;
		}
	}
}; //end of class delegate
System_Void System_Action_ctor(System_Action* _delegate, System_Void* object, std::function<System_Void(System_Void*) > fn){
	_delegate->Register(fn, object);
}
System_Void System_Action_Invoke( System_Action* _delegate){
	_delegate->Invoke();
}

System_Void _NBody_Main();

System_Void ___c__DisplayClass1_ctor(___c__DisplayClass1 * _this);

/*System_Void System_Action_ctor(System_Action * _this, System_Object * object, System_IntPtr  method);

System_Void System_Action_Invoke(System_Action * _this);*/

System_Void ___c__DisplayClass1__Main_b__0(___c__DisplayClass1 * _this);

System_Int32 _NBody_AddPrimes(System_Int32 len);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

typedef System_Void(*System_Action_InvokeVirtPtr)(const System_Action* _this);
System_Void System_Action_Invoke_vcall(System_Action *_this);
System_Void System_Action_Invoke_vcall(System_Action *_this){
	switch (_this->_typeId)
	{
	case 4:
		System_Action_Invoke(_this);
		return;
	}
}
// --- End of definition of virtual method tables ---

System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{
	printf("%ls\n", value.get()->Text->Items);
}
System_Void System_Console_Write(System_Int32 value)
{
	printf("%d", value);
}
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{
	System_Action * local_0;
	___c__DisplayClass1 * local_1;
	___c__DisplayClass1 * vreg_2;
	System_Void(*vreg_3)(___c__DisplayClass1*);
	System_Action * vreg_5;

	___c__DisplayClass1  vreg_1;
	vreg_1._typeId = 2;
	___c__DisplayClass1_ctor(&vreg_1);
	vreg_2 = &vreg_1;
	local_1 = vreg_2;
	System_Console_WriteLine(_str(0));
	local_1->len = 1000000;
	vreg_3 = &(___c__DisplayClass1__Main_b__0);

	System_Action  vreg_4;
	vreg_4._typeId = 4;
	System_Action_ctor(&vreg_4, local_1, (System_Void(*)(System_Void*))vreg_3);
	vreg_5 = &vreg_4;
	local_0 = vreg_5;
	System_Action_Invoke(local_0);
	return;
}


System_Void ___c__DisplayClass1_ctor(___c__DisplayClass1 * _this)

{

	return;
}

/*
System_Void System_Action_ctor(System_Action * _this, System_Object * object, System_IntPtr  method)
{
	std::shared_ptr<System_Action> ptr = std::make_shared<System_Action>((System_Action&)_this);
	System_Action_ctor(ptr,object,method);
}


System_Void System_Action_Invoke(System_Action * _this)

{
	std::shared_ptr<System_Action> ptr = std::make_shared<System_Action>(*_this);
	//System_Action_Invoke(ptr);
}
*/

System_Void ___c__DisplayClass1__Main_b__0(___c__DisplayClass1 * _this)

{
	System_Int32 local_0;
	System_Int32 vreg_1;
	System_Int32 vreg_2;

	vreg_1 = _this->len;
	vreg_2 = _NBody_AddPrimes(vreg_1);
	local_0 = vreg_2;
	System_Console_Write(local_0);
	System_Console_WriteLine(_str(1));
	return;
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
	vreg_1 = local_1 % 2;
	if (!(vreg_1)) goto label_2B;
	local_2 = 1;
	local_3 = 2;
	goto label_1E;
	vreg_2 = local_1%local_3;
	if (vreg_2) goto label_1A;
	local_2 = 0;
	goto label_24;
label_1A:
	vreg_3 = local_3 + 1;
	local_3 = vreg_3;
label_1E:
	vreg_4 = local_3*local_3;
	if (local_1 <= vreg_4) goto label_1A;
label_24:
	if (!(local_2)) goto label_2B;
	vreg_5 = local_0 + 1;
	local_0 = vreg_5;
label_2B:
	vreg_6 = local_1 + 1;
	local_1 = vreg_6;
label_2F:
	if (len<local_1) goto label_6;
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

