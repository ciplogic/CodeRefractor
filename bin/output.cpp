#include "sloth.h"
#include "runtime_base.partcpp"
#include "stdio.h"
void System_Console__WriteLine(System::Int32 value)
{ printf("%d\n", value); }
namespace SimpleAdditions {
struct MainClass;
}
namespace SimpleAdditions {
struct MainClass {
}; }
void SimpleAdditions_MainClass__Main();
void SimpleAdditions_MainClass__MainClass_ctor(const std::shared_ptr<SimpleAdditions::MainClass>& _this);
void SimpleAdditions_MainClass__Logic2(const std::shared_ptr<SimpleAdditions::MainClass>& _this, System::Int32 value);
void SimpleAdditions_MainClass__Main()
{
std::shared_ptr<SimpleAdditions::MainClass> local_0;
std::shared_ptr<SimpleAdditions::MainClass> vreg_1;
std::shared_ptr<SimpleAdditions::MainClass> vreg_2;
System::Int32 vreg_3;

vreg_1 = std::shared_ptr<SimpleAdditions::MainClass>(new SimpleAdditions::MainClass());
SimpleAdditions_MainClass__MainClass_ctor(vreg_1);
local_0 = vreg_1;
vreg_2 = local_0;
vreg_3 = 2;
SimpleAdditions_MainClass__Logic2(vreg_2, vreg_3);
return;
}


void SimpleAdditions_MainClass__MainClass_ctor(const std::shared_ptr<SimpleAdditions::MainClass>& _this)
{
std::shared_ptr<SimpleAdditions::MainClass> vreg_1;

vreg_1 = _this;

return;
}


void SimpleAdditions_MainClass__Logic2(const std::shared_ptr<SimpleAdditions::MainClass>& _this, System::Int32 value)
{
System::Int32 local_0;
System::Int32 vreg_1;
System::Int32 vreg_2;
System::Int32 vreg_3;
System::Int32 vreg_4;
System::Int32 vreg_5;
System::Int32 vreg_6;
System::Int32 vreg_7;

vreg_1 = value;
local_0 = vreg_1;
vreg_2 = local_0;
vreg_3 = 3;
vreg_4 = vreg_2-vreg_3;
switch(vreg_4) {
case 0:	goto label_25;
case 1:	goto label_43;
case 2:	goto label_34;
}

goto label_43;
label_25:
vreg_5 = 1;
System_Console__WriteLine(vreg_5);
goto label_52;
label_34:
vreg_6 = 0;
System_Console__WriteLine(vreg_6);
goto label_52;
label_43:
vreg_7 = value;
System_Console__WriteLine(vreg_7);
goto label_52;
label_52:
return;
}


int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
SimpleAdditions_MainClass__Main();
return 0;
}
void mapLibs() {
}

void RuntimeHelpersBuildConstantTable() {
}

