#include "sloth.h"
struct SimpleAdditions_NBody; 
struct SimpleAdditions_NBody {
};
System_Void SimpleAdditions_NBody_Main();

System_Void SimpleAdditions_NBody_FillWithColor(System_UInt32*  data, System_Int32 w, System_Int32 h, System_UInt32 color);

#include "runtime_base.partcpp"
///---Begin closure code --- 
System_Void SimpleAdditions_NBody_Main()
{
std::shared_ptr< Array < System_UInt32 > > local_0;
System_UInt32*  local_1;
std::shared_ptr< Array < System_UInt32 > > local_2;
System_Int32 vreg_1;
std::shared_ptr< Array < System_UInt32 > > vreg_2;
std::shared_ptr< Array < System_UInt32 > > vreg_3;
std::shared_ptr< Array < System_UInt32 > > vreg_4;
std::shared_ptr< Array < System_UInt32 > > vreg_5;
System_Int32 vreg_6;
System_Int32 vreg_7;
System_Int32 vreg_8;
std::shared_ptr< Array < System_UInt32 > > vreg_9;
System_Int32 vreg_10;
System_UInt32*  vreg_11;
System_UInt32*  vreg_12;
System_IntPtr vreg_13;
System_Int32 vreg_14;
System_Int32 vreg_15;
System_Int32 vreg_16;
System_Int32 vreg_17;

vreg_1 = 480000;
vreg_2 = std::make_shared< Array < System_UInt32 > >(480000); 
local_0 = vreg_2;
vreg_3 = vreg_2;
vreg_4 = vreg_2;
local_2 = vreg_2;
if(!(vreg_3)) goto label_22;
vreg_5 = local_2;
vreg_6 = vreg_5->Length;
vreg_7 = (int)vreg_6;
if(vreg_7) goto label_27;
label_22:
vreg_8 = 0;
local_1 = 0;
goto label_35;
label_27:
vreg_9 = local_2;
vreg_10 = 0;
vreg_11 = & (vreg_9->Items[vreg_10]);
local_1 = vreg_11;
label_35:
vreg_12 = local_1;
vreg_13 = (void*)vreg_12;
SimpleAdditions_NBody_FillWithColor((System_UInt32*)vreg_13, 800, 600, 255);
return;
}


System_Void SimpleAdditions_NBody_FillWithColor(System_UInt32*  data, System_Int32 w, System_Int32 h, System_UInt32 color)
{
System_Int32 local_0;
System_Int32 local_1;
System_Boolean local_2;
System_Int32 vreg_1;
System_Int32 vreg_2;
System_Int32 vreg_3;
System_Int32 vreg_4;
System_UInt32*  vreg_5;
System_UInt32 vreg_6;
System_UInt32*  vreg_7;
System_Int32 vreg_8;
System_IntPtr vreg_9;
System_UInt32*  vreg_10;
System_Int32 vreg_11;
System_Int32 vreg_12;
System_Int32 vreg_13;
System_Int32 vreg_14;
System_Int32 vreg_15;
System_Int32 vreg_16;
System_Boolean vreg_17;

vreg_1 = w;
vreg_2 = h;
vreg_3 = vreg_1*h;
vreg_4 = 0;
local_1 = 0;
vreg_12 = 1;
vreg_9 = (void*)4;
vreg_8 = 4;
vreg_7 = data;
vreg_6 = color;
vreg_5 = data;
vreg_10 = vreg_7+(size_t)vreg_9;
local_0 = *vreg_10;
vreg_15 = *vreg_10;
goto label_24;
label_9:
vreg_6 = *vreg_5;
vreg_11 = local_1;
vreg_13 = local_1+1;
local_1 = vreg_13;
label_24:
vreg_14 = local_1;
vreg_16 = (local_1 < vreg_15)?1:0;
local_2 = vreg_16;
vreg_17 = vreg_16;
if(vreg_16) goto label_9;
return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
SimpleAdditions_NBody_Main();
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

