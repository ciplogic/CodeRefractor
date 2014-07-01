#include "sloth.h"
#include <functional>
struct System_Object;
struct _Box;
struct System_Console;
struct _IEnglishDimensions;
struct _IMetricDimensions;
struct System_ValueType;
struct System_String;
struct System_IComparable{};

struct System_ICloneable{};

struct System_IConvertible{};

struct System_IComparable_1{};

struct System_Collections_Generic_IEnumerable_1{};

struct System_Collections_IEnumerable{};

struct System_IEquatable_1{};
struct System_Object {
	int _typeId;
};
struct _Box : public System_Object {
	System_Single lengthInches;
	System_Single widthInches;
};
struct System_Console : public System_Object {
};
struct _IEnglishDimensions : public System_Object {
};
struct _IMetricDimensions : public System_Object {
};
struct System_ValueType : public System_Object {
};
struct System_String : public System_Object {
	std::shared_ptr< Array < System_Char > > Text;
};

System_Void _Box_Main();

System_Void _Box_ctor(const std::shared_ptr<_Box>& _this, System_Single length, System_Single width);

System_Void CodeRefactor_OpenRuntime_CrConsole_WriteLine(System_Single value);

System_Single _Box_IEnglishDimensions_Length(const std::shared_ptr<_Box>& _this);

System_Single _Box_IEnglishDimensions_Width(const std::shared_ptr<_Box>& _this);

System_Single _Box_IMetricDimensions_Length(const std::shared_ptr<_Box>& _this);

System_Single _Box_IMetricDimensions_Width(const std::shared_ptr<_Box>& _this);

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

System_Single _IEnglishDimensions_Length_vcall(const std::shared_ptr<System_Object> _this);
System_Single _IEnglishDimensions_Width_vcall(const std::shared_ptr<System_Object> _this);
System_Single _IMetricDimensions_Length_vcall(const std::shared_ptr<System_Object> _this);
System_Single _IMetricDimensions_Width_vcall(const std::shared_ptr<System_Object> _this);
System_Single _IEnglishDimensions_Length_vcall(const std::shared_ptr<System_Object> _this){
	switch (_this->_typeId)
	{
	case 1:
		return _Box_IEnglishDimensions_Length(std::static_pointer_cast<_Box>(_this));
	}
}
System_Single _IEnglishDimensions_Width_vcall(const std::shared_ptr<System_Object> _this){
	switch (_this->_typeId)
	{
	case 1:
		return _Box_IEnglishDimensions_Width(std::static_pointer_cast<_Box>(_this));
	}
}
System_Single _IMetricDimensions_Length_vcall(const std::shared_ptr<System_Object> _this){
	switch (_this->_typeId)
	{
	case 1:
		return _Box_IMetricDimensions_Length(std::static_pointer_cast<_Box>(_this));
	}
}
System_Single _IMetricDimensions_Width_vcall(const std::shared_ptr<System_Object> _this){
	switch (_this->_typeId)
	{
	case 1:
		return _Box_IMetricDimensions_Width(std::static_pointer_cast<_Box>(_this));
	}
}
// --- End of definition of virtual method tables ---

System_Void System_Console_WriteLine(System_Double value)
{
	printf("%lf\n", value);
}
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _Box_Main()

{
	std::shared_ptr<_Box> local_0;
	std::shared_ptr<System_Object> local_1;
	std::shared_ptr<System_Object> local_2;
	std::shared_ptr<_Box> vreg_1;
	std::shared_ptr<_Box> vreg_2;
	System_Single vreg_3;
	System_Single vreg_4;
	System_Single vreg_5;
	System_Single vreg_6;

	vreg_1 = std::make_shared<_Box >();
	vreg_1->_typeId = 1;
	_Box_ctor(vreg_1, 30, 20);
	vreg_2 = vreg_1;
	local_0 = vreg_2;
	local_1 = local_0;
	local_2 = local_0;
	vreg_3 = _IEnglishDimensions_Length_vcall(local_1);
	System_Console_WriteLine(vreg_3);
	vreg_4 = _IEnglishDimensions_Width_vcall(local_1);
	System_Console_WriteLine(vreg_4);
	vreg_5 = _IMetricDimensions_Length_vcall(local_2);
	System_Console_WriteLine(vreg_5);
	vreg_6 = _IMetricDimensions_Width_vcall(local_2);
	System_Console_WriteLine(vreg_6);
	return;
}


System_Void _Box_ctor(const std::shared_ptr<_Box>& _this, System_Single length, System_Single width)

{

	_this->lengthInches = length;
	_this->widthInches = width;
	return;
}


System_Void CodeRefactor_OpenRuntime_CrConsole_WriteLine(System_Single value)

{
	System_Double vreg_1;

	vreg_1 = (double)value;
	System_Console_WriteLine(vreg_1);
	return;
}


System_Single _Box_IEnglishDimensions_Length(const std::shared_ptr<_Box>& _this)

{
	System_Single vreg_1;

	vreg_1 = _this->lengthInches;
	return vreg_1;
}


System_Single _Box_IEnglishDimensions_Width(const std::shared_ptr<_Box>& _this)

{
	System_Single vreg_1;

	vreg_1 = _this->widthInches;
	return vreg_1;
}


System_Single _Box_IMetricDimensions_Length(const std::shared_ptr<_Box>& _this)

{
	System_Single vreg_1;
	System_Single vreg_2;

	vreg_1 = _this->lengthInches;
	vreg_2 = vreg_1*2.53999996185303;
	return vreg_2;
}


System_Single _Box_IMetricDimensions_Width(const std::shared_ptr<_Box>& _this)

{
	System_Single vreg_1;
	System_Single vreg_2;

	vreg_1 = _this->widthInches;
	vreg_2 = vreg_1*2.53999996185303;
	return vreg_2;
}


///---End closure code --- 
System_Void initializeRuntime();
int main(int argc, char**argv) {
	auto argsAsList = System_getArgumentsAsList(argc, argv);
	initializeRuntime();
	_Box_Main();
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

