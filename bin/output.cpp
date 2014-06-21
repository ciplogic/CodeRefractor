#include "sloth.h"
#include <functional>
struct System_Object;
struct Obj_Test;
struct Obj_A;
struct Obj_B;
struct System_Object {
	int _typeId;
};
struct Obj_Test : public System_Object {
};
struct Obj_A : public System_Object {
};
struct Obj_B : public Obj_A {
};

System_Int32 Obj_Test_Main();

System_Void Obj_B_ctor(const std::shared_ptr<Obj_B>& _this);

System_Int32 Obj_B_F(const std::shared_ptr<Obj_B>& _this);

System_Int32 Obj_B_H(const std::shared_ptr<Obj_B>& _this);

System_Int32 Obj_A_F(const std::shared_ptr<Obj_A>& _this);

System_Int32 Obj_A_G(const std::shared_ptr<Obj_A>& _this);

System_Int32 Obj_A_H(const std::shared_ptr<Obj_A>& _this);

System_Int32 Obj_B_G(const std::shared_ptr<Obj_B>& _this);

System_Void Obj_A_ctor(const std::shared_ptr<Obj_A>& _this);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

typedef System_Int32(*Obj_A_GVirtPtr)(const std::shared_ptr<Obj_A> _this);
System_Int32 Obj_A_G_vcall(const std::shared_ptr<Obj_A> _this);
typedef System_Int32(*Obj_A_HVirtPtr)(const std::shared_ptr<Obj_A> _this);
System_Int32 Obj_A_H_vcall(const std::shared_ptr<Obj_A> _this);
System_Int32 Obj_A_G_vcall(const std::shared_ptr<Obj_A> _this){
	switch (_this->_typeId)
	{
	case 2:
		return Obj_A_G(std::static_pointer_cast<Obj_A>(_this));
	case 3:
		return Obj_B_G(std::static_pointer_cast<Obj_B>(_this));
	}
}
System_Int32 Obj_A_H_vcall(const std::shared_ptr<Obj_A> _this){
	switch (_this->_typeId)
	{
	case 2:
		return Obj_A_H(std::static_pointer_cast<Obj_A>(_this));
	case 3:
		return Obj_B_H(std::static_pointer_cast<Obj_B>(_this));
	}
}
// --- End of definition of virtual method tables ---

///--- PInvoke code --- 
///---Begin closure code --- 
System_Int32 Obj_Test_Main()

{
	System_Int32 local_0;
	std::shared_ptr<Obj_B> local_1;
	std::shared_ptr<Obj_A> local_2;
	std::shared_ptr<Obj_B> vreg_1;
	std::shared_ptr<Obj_B> vreg_2;
	System_Int32 vreg_3;
	System_Int32 vreg_4;
	System_Int32 vreg_5;
	System_Int32 vreg_6;
	System_Int32 vreg_7;
	System_Int32 vreg_8;
	System_Int32 vreg_9;
	System_Int32 vreg_10;
	System_Int32 vreg_11;
	System_Int32 vreg_12;
	System_Int32 vreg_13;
	System_Int32 vreg_14;
	System_Int32 vreg_15;
	System_Int32 vreg_16;
	std::shared_ptr<Obj_B> vreg_17;
	System_Int32 vreg_18;
	System_Int32 vreg_19;

	local_0 = 0;
	vreg_1 = std::make_shared<Obj_B >();
	vreg_1->_typeId = 3;
	Obj_B_ctor(vreg_1);
	vreg_2 = vreg_1;
	local_1 = vreg_2;
	local_2 = local_1;
	vreg_3 = Obj_A_F(local_2);
	if (vreg_3 == 1) goto label_17;
	vreg_4 = local_0 | 1;
	local_0 = vreg_4;
label_17:
	vreg_5 = Obj_B_F(local_1);
	if (vreg_5 == 3) goto label_24;
	vreg_6 = local_0 | 2;
	local_0 = vreg_6;
label_24:
	vreg_7 = Obj_A_G_vcall(local_1);
	if (vreg_7 == 4) goto label_31;
	vreg_8 = local_0 | 4;
	local_0 = vreg_8;
label_31:
	vreg_9 = Obj_A_G_vcall(local_2);
	if (vreg_9 == 4) goto label_3E;
	vreg_10 = local_0 | 8;
	local_0 = vreg_10;
label_3E:
	vreg_11 = Obj_A_H_vcall(local_2);
	if (vreg_11 == 10) goto label_4D;
	vreg_12 = local_0 | 16;
	local_0 = vreg_12;
label_4D:
	vreg_13 = Obj_B_H(local_1);
	if (vreg_13 == 11) goto label_5C;
	vreg_14 = local_0 | 32;
	local_0 = vreg_14;
label_5C:
	vreg_15 = Obj_A_H_vcall(local_1);
	if (vreg_15 == 10) goto label_6B;
	vreg_16 = local_0 | 64;
	local_0 = vreg_16;
label_6B:
	vreg_17 = std::static_pointer_cast<Obj_B >(local_2);
	vreg_18 = Obj_B_H(vreg_17);
	if (vreg_18 == 11) goto label_82;
	vreg_19 = local_0 | 128;
	local_0 = vreg_19;
label_82:
	return local_0;
}


System_Void Obj_B_ctor(const std::shared_ptr<Obj_B>& _this)

{

	Obj_A_ctor(_this);
	return;
}


System_Int32 Obj_B_F(const std::shared_ptr<Obj_B>& _this)

{

	return 3;
}


System_Int32 Obj_B_H(const std::shared_ptr<Obj_B>& _this)

{

	return 11;
}


System_Int32 Obj_A_F(const std::shared_ptr<Obj_A>& _this)

{

	return 1;
}


System_Int32 Obj_A_G(const std::shared_ptr<Obj_A>& _this)

{

	return 2;
}


System_Int32 Obj_A_H(const std::shared_ptr<Obj_A>& _this)

{

	return 10;
}


System_Int32 Obj_B_G(const std::shared_ptr<Obj_B>& _this)

{

	return 4;
}


System_Void Obj_A_ctor(const std::shared_ptr<Obj_A>& _this)

{

	return;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
	auto argsAsList = System_getArgumentsAsList(argc, argv);
	initializeRuntime();
	return Obj_Test_Main();
	return 0;
}
System_Void mapLibs() {
}

System_Void RuntimeHelpersBuildConstantTable() {
}

System_Void buildStringTable() {
	_AddJumpAndLength(0, 15);
} // buildStringTable
const wchar_t _stringTable[16] = {
	80, 114, 105, 109, 101, 32, 110, 117, 109, 98, 101, 114, 115, 58, 32, 0 /* "Prime numbers: " */
}; // _stringTable 

