#include "sloth.h"
#include "runtime_base.partcpp"
#include "stdio.h"
void System_Console__WriteLine(System::Int32 value)
{ printf("%d\n", value); }
namespace  {
struct NBody;
}
namespace  {
struct Test;
}
namespace  {
struct NBody {
}; }
void _NBody__Main();
namespace  {
struct Test {
}; }
void _Test__Test_ctor(const std::shared_ptr<Test>& _this);
void _Test__F1(const std::shared_ptr<Test>& _this);
System::Int32 _Test__F2(System::Int32 a, System::Int32 b);
void _NBody__Main()
{
std::shared_ptr<Test> local_0;
System::Int32 local_1;
std::shared_ptr<Test> vreg_1;
std::shared_ptr<Test> vreg_2;
System::Int32 vreg_3;
System::Int32 vreg_4;
System::Int32 vreg_5;
System::Int32 vreg_6;

vreg_1 = std::shared_ptr<Test>(new Test());
_Test__Test_ctor(vreg_1);
local_0 = vreg_1;
vreg_2 = local_0;
vreg_3 = 2;
vreg_4 = 4;
vreg_3 = _Test__F2(vreg_3, vreg_4);
local_1 = vreg_5;
vreg_6 = local_1;
System_Console__WriteLine(vreg_6);
return;
}


void _Test__Test_ctor(const std::shared_ptr<Test>& _this)
{
std::shared_ptr<Test> vreg_1;

vreg_1 = _this;

return;
}


void _Test__F1(const std::shared_ptr<Test>& _this)
{
System::Int32 local_0;
System::Int32 local_1;
System::Int32 vreg_1;
System::Int32 vreg_2;
System::Int32 vreg_3;
System::Int32 vreg_4;
System::Int32 vreg_5;
System::Int32 vreg_6;
System::Int32 vreg_7;
System::Int32 vreg_8;

vreg_3 = 2;
vreg_4 = 4;
vreg_5 = vreg_7;
vreg_6 = vreg_8;
vreg_3 = vreg_1+vreg_2;
local_1 = vreg_7;
goto label_7;
label_7:
vreg_8 = local_1;
vreg_3 = vreg_4;
return vreg_4;
local_1 = vreg_5;
vreg_6 = local_1;
System_Console__WriteLine(vreg_6);
return;
}


System::Int32 _Test__F2(System::Int32 a, System::Int32 b)
{
System::Int32 local_0;
System::Int32 vreg_1;
System::Int32 vreg_2;
System::Int32 vreg_3;
System::Int32 vreg_4;

vreg_5 = vreg_7;
vreg_6 = vreg_8;
vreg_3 = vreg_1+vreg_2;
local_1 = vreg_7;
goto label_7;
label_7:
vreg_8 = local_1;
return vreg_4;
}


int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
_NBody__Main();
return 0;
}
void mapLibs() {
}

void RuntimeHelpersBuildConstantTable() {
}

