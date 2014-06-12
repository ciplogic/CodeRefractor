#include "sloth.h"
struct System_Object; 
struct System_Math; 
struct System_ValueType; 
struct _NBody; 
struct _Body; 
struct _Pair; 
struct _NBodySystem; 
struct System_Console; 
struct System_String; 
struct System_Object {
int _typeId;
};
struct System_Math : public System_Object {
};
struct System_ValueType : public System_Object {
};
struct _NBody : public System_Object {
};
struct _Body : public System_Object {
 System_Double mass;
 System_Double x;
 System_Double y;
 System_Double z;
 System_Double vx;
 System_Double vy;
 System_Double vz;
};
struct _Pair : public System_Object {
 std::shared_ptr<_Body> bi;
 std::shared_ptr<_Body> bj;
};
struct _NBodySystem : public System_Object {
 std::shared_ptr< Array < std::shared_ptr<_Body> > > bodies;
 std::shared_ptr< Array < std::shared_ptr<_Pair> > > pairs;
};
struct System_Console : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};

System_Void _NBody_Main();

System_Void _NBodySystem_ctor(_NBodySystem * _this);

System_Double _NBodySystem_Energy(_NBodySystem * _this);

System_Void _NBodySystem_Advance(_NBodySystem * _this, System_Double  dt);

System_Void _Body_ctor();

System_Void _Pair_ctor();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void System_Console_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
System_Void System_Console_WriteLine(System_Double value)
{ printf("%lf\n", value); }
#include "math.h"
System_Double System_Math_Sqrt(System_Double d)
{ return sqrt(d); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{
_NBodySystem * local_1;
System_Int32 local_2;
System_Boolean local_3;
System_Double vreg_3;
System_Double vreg_6;

System_Console_WriteLine(_str(0));
_NBodySystem  vreg_1;
vreg_1._typeId = 0;
_NBodySystem_ctor(&vreg_1);
local_1 = &vreg_1;
vreg_3 = _NBodySystem_Energy(&vreg_1);
System_Console_WriteLine(vreg_3);
local_2 = 0;
goto label_3C;
label_28:
_NBodySystem_Advance(local_1, 0.01);
local_2 = local_2+1;
label_3C:
local_3 = (local_2 < 500000)?1:0;
if(local_3) goto label_28;
vreg_6 = _NBodySystem_Energy(local_1);
System_Console_WriteLine(vreg_6);
return;
}


System_Void _NBodySystem_ctor(_NBodySystem * _this)

{
System_Int32 local_0;
System_Int32 local_1;
System_Int32 local_2;
System_Double local_3;
System_Double local_4;
System_Double local_5;
_Body * local_6;
_Body * local_7;
std::shared_ptr< Array < std::shared_ptr<_Body> > > local_13;
System_Boolean local_14;
Array < std::shared_ptr<_Body> > * local_15;
System_Int32 local_16;
std::shared_ptr<_Body> vreg_2;
std::shared_ptr<_Body> vreg_4;
std::shared_ptr<_Body> vreg_6;
std::shared_ptr<_Body> vreg_8;
std::shared_ptr<_Body> vreg_10;
System_Int32 vreg_18;
System_Int32 vreg_19;
System_Int32 vreg_20;
std::shared_ptr< Array < std::shared_ptr<_Pair> > > vreg_21;
std::shared_ptr<_Pair> vreg_24;
_Pair * vreg_27;
std::shared_ptr<_Body> vreg_29;
_Pair * vreg_31;
std::shared_ptr<_Body> vreg_33;
System_Int32 vreg_38;
System_Int32 vreg_44;
System_Double vreg_48;
System_Double vreg_50;
System_Double vreg_52;
System_Double vreg_54;
System_Double vreg_56;
System_Double vreg_58;
System_Int32 vreg_61;
System_Int32 vreg_62;
Array < std::shared_ptr<_Body> > * vreg_64;
System_Double vreg_66;
System_Double vreg_67;
System_Double vreg_68;
System_Double vreg_69;
System_Double vreg_70;
System_Double vreg_71;
Array < std::shared_ptr<_Pair> > * vreg_76;
Array < std::shared_ptr<_Body> > * vreg_77;
System_Double vreg_79;
System_Int32 vreg_84;
System_Int32 vreg_85;
Array < std::shared_ptr<_Body> > * vreg_86;

local_13 = std::make_shared< Array <std::shared_ptr<_Body>> >(5); 
vreg_2 = std::make_shared<_Body >();
vreg_2->_typeId = 4;
vreg_2->mass = 39.4784176043574;
(*local_13)[0] = vreg_2; 
vreg_4 = std::make_shared<_Body >();
vreg_4->_typeId = 4;
vreg_4->x = 4.84143144246472;
vreg_4->y = -1.16032004402743;
vreg_4->z = -0.103622044471123;
vreg_4->vx = 0.606326392995832;
vreg_4->vy = 2.81198684491626;
vreg_4->vz = -0.0252183616598876;
vreg_4->mass = 0.0376936748703895;
(*local_13)[1] = vreg_4; 
vreg_6 = std::make_shared<_Body >();
vreg_6->_typeId = 4;
vreg_6->x = 8.34336671824458;
vreg_6->y = 4.1247985641243;
vreg_6->z = -0.403523417114321;
vreg_6->vx = -1.01077434617879;
vreg_6->vy = 1.82566237123041;
vreg_6->vz = 0.00841576137658415;
vreg_6->mass = 0.0112863261319688;
(*local_13)[2] = vreg_6; 
vreg_8 = std::make_shared<_Body >();
vreg_8->_typeId = 4;
vreg_8->x = 12.8943695621391;
vreg_8->y = -15.1111514016986;
vreg_8->z = -0.223307578892656;
vreg_8->vx = 1.08279100644154;
vreg_8->vy = 0.868713018169608;
vreg_8->vz = -0.0108326374013636;
vreg_8->mass = 0.00172372405705971;
(*local_13)[3] = vreg_8; 
vreg_10 = std::make_shared<_Body >();
vreg_10->_typeId = 4;
vreg_10->x = 15.3796971148509;
vreg_10->y = -25.919314609988;
vreg_10->z = 0.179258772950371;
vreg_10->vx = 0.979090732243898;
vreg_10->vy = 0.594698998647676;
vreg_10->vz = -0.0347559555040781;
vreg_10->mass = 0.00203368686992463;
(*local_13)[4] = vreg_10; 
_this->bodies = local_13;
vreg_86 = _this->bodies.get();
vreg_84 = vreg_86->Length;
vreg_85 = (int)vreg_84;
vreg_18 = vreg_85-1;
vreg_19 = vreg_85*vreg_18;
vreg_20 = vreg_19/2;
vreg_21 = std::make_shared< Array <std::shared_ptr<_Pair>> >(vreg_20); 
_this->pairs = vreg_21;
local_0 = 0;
local_1 = 0;
vreg_77 = vreg_86;
vreg_76 = _this->pairs.get();
vreg_38 = vreg_85;
vreg_44 = vreg_85-1;
goto label_2AB;
label_24F:
local_2 = local_1+1;
goto label_296;
label_255:
vreg_24 = std::make_shared<_Pair >();
vreg_24->_typeId = 1;
(*vreg_76)[local_0] = vreg_24; 
vreg_27 = ((*vreg_76)[local_0]).get();
vreg_29 = (*vreg_77)[local_1];
vreg_27->bi = vreg_29;
vreg_31 = ((*vreg_76)[local_0]).get();
vreg_33 = (*vreg_77)[local_2];
vreg_31->bj = vreg_33;
local_0 = local_0+1;
local_2 = local_2+1;
label_296:
local_14 = (local_2 < vreg_38)?1:0;
if(local_14) goto label_255;
local_1 = local_1+1;
label_2AB:
local_14 = (local_1 < vreg_44)?1:0;
if(local_14) goto label_24F;
local_3 = 0;
local_4 = 0;
local_5 = 0;
local_15 = _this->bodies.get();
local_16 = 0;
vreg_61 = local_15->Length;
vreg_62 = (int)vreg_61;
goto label_335;
label_2EC:
local_6 = ((*local_15)[local_16]).get();
vreg_48 = local_6->vx;
vreg_79 = local_6->mass;
vreg_50 = vreg_48*vreg_79;
local_3 = local_3+vreg_50;
vreg_52 = local_6->vy;
vreg_54 = vreg_52*vreg_79;
local_4 = local_4+vreg_54;
vreg_56 = local_6->vz;
vreg_58 = vreg_56*vreg_79;
local_5 = local_5+vreg_58;
local_16 = local_16+1;
label_335:
local_14 = (local_16 < vreg_62)?1:0;
if(local_14) goto label_2EC;
vreg_64 = _this->bodies.get();
local_7 = ((*vreg_64)[0]).get();
vreg_66 = -local_3;
vreg_67 = vreg_66/39.4784176043574;
local_7->vx = vreg_67;
vreg_68 = -local_4;
vreg_69 = vreg_68/39.4784176043574;
local_7->vy = vreg_69;
vreg_70 = -local_5;
vreg_71 = vreg_70/39.4784176043574;
local_7->vz = vreg_71;
return;
}


System_Double _NBodySystem_Energy(_NBodySystem * _this)

{
System_Double local_0;
System_Int32 local_1;
System_Int32 local_3;
Array < std::shared_ptr<_Body> > * vreg_1;
_Body * vreg_2;
System_Double vreg_4;
System_Double vreg_7;
System_Double vreg_10;
System_Double vreg_11;
System_Double vreg_14;
System_Double vreg_15;
System_Double vreg_16;
Array < std::shared_ptr<_Body> > * vreg_19;
_Body * vreg_20;
System_Double vreg_21;
System_Double vreg_22;
System_Double vreg_23;
System_Double vreg_24;
System_Double vreg_25;
System_Double vreg_26;
System_Double vreg_27;
System_Double vreg_28;
System_Double vreg_29;
System_Double vreg_30;
System_Double vreg_31;
System_Double vreg_32;
System_Double vreg_33;
System_Double vreg_34;
System_Double vreg_35;
System_Double vreg_36;
System_Double vreg_37;
System_Double vreg_38;
System_Double vreg_39;
System_Int32 vreg_44;
System_Int32 vreg_45;
System_Int32 vreg_49;
System_Int32 vreg_50;
System_Double vreg_51;
System_Double vreg_52;
System_Double vreg_53;
System_Int32 vreg_55;
System_Int32 vreg_56;
Array < std::shared_ptr<_Body> > * vreg_58;
System_Double vreg_59;

local_0 = 0;
local_1 = 0;
vreg_58 = _this->bodies.get();
vreg_19 = vreg_58;
vreg_1 = vreg_58;
vreg_55 = vreg_58->Length;
vreg_56 = (int)vreg_55;
vreg_49 = vreg_56;
vreg_44 = vreg_56;
goto label_DD;
label_12:
vreg_2 = ((*vreg_1)[local_1]).get();
vreg_59 = vreg_2->mass;
vreg_4 = 0.5*vreg_59;
vreg_51 = vreg_2->vx;
vreg_7 = vreg_51*vreg_51;
vreg_52 = vreg_2->vy;
vreg_10 = vreg_52*vreg_52;
vreg_11 = vreg_7+vreg_10;
vreg_53 = vreg_2->vz;
vreg_14 = vreg_53*vreg_53;
vreg_15 = vreg_11+vreg_14;
vreg_16 = vreg_4*vreg_15;
local_0 = local_0+vreg_16;
local_3 = local_1+1;
vreg_30 = vreg_59;
vreg_27 = vreg_2->z;
vreg_24 = vreg_2->y;
vreg_21 = vreg_2->x;
goto label_C7;
label_5F:
vreg_20 = ((*vreg_19)[local_3]).get();
vreg_22 = vreg_20->x;
vreg_23 = vreg_21-vreg_22;
vreg_25 = vreg_20->y;
vreg_26 = vreg_24-vreg_25;
vreg_28 = vreg_20->z;
vreg_29 = vreg_27-vreg_28;
vreg_31 = vreg_20->mass;
vreg_32 = vreg_30*vreg_31;
vreg_33 = vreg_23*vreg_23;
vreg_34 = vreg_26*vreg_26;
vreg_35 = vreg_33+vreg_34;
vreg_36 = vreg_29*vreg_29;
vreg_37 = vreg_35+vreg_36;
vreg_38 = System_Math_Sqrt(vreg_37);
vreg_39 = vreg_32/vreg_38;
local_0 = local_0-vreg_39;
local_3 = local_3+1;
label_C7:
vreg_45 = (local_3 < vreg_44)?1:0;
if(vreg_45) goto label_5F;
local_1 = local_1+1;
label_DD:
vreg_50 = (local_1 < vreg_49)?1:0;
if(vreg_50) goto label_12;
return local_0;
}


System_Void _NBodySystem_Advance(_NBodySystem * _this, System_Double  dt)

{
_Pair * local_0;
_Body * local_1;
_Body * local_2;
System_Double local_3;
System_Double local_4;
System_Double local_5;
System_Double local_6;
System_Double local_7;
_Body * local_8;
Array < std::shared_ptr<_Pair> > * local_9;
System_Int32 local_10;
System_Boolean local_11;
Array < std::shared_ptr<_Body> > * local_12;
System_Double vreg_5;
System_Double vreg_6;
System_Double vreg_8;
System_Double vreg_9;
System_Double vreg_11;
System_Double vreg_12;
System_Double vreg_14;
System_Double vreg_15;
System_Double vreg_16;
System_Double vreg_17;
System_Double vreg_19;
System_Double vreg_20;
System_Double vreg_23;
System_Double vreg_25;
System_Double vreg_26;
System_Double vreg_27;
System_Double vreg_29;
System_Double vreg_31;
System_Double vreg_32;
System_Double vreg_33;
System_Double vreg_35;
System_Double vreg_37;
System_Double vreg_38;
System_Double vreg_39;
System_Double vreg_41;
System_Double vreg_43;
System_Double vreg_44;
System_Double vreg_45;
System_Double vreg_47;
System_Double vreg_49;
System_Double vreg_50;
System_Double vreg_51;
System_Double vreg_53;
System_Double vreg_55;
System_Double vreg_56;
System_Double vreg_57;
System_Int32 vreg_59;
System_Int32 vreg_60;
System_Double vreg_65;
System_Double vreg_66;
System_Double vreg_67;
System_Double vreg_68;
System_Double vreg_70;
System_Double vreg_71;
System_Double vreg_72;
System_Double vreg_73;
System_Double vreg_75;
System_Double vreg_76;
System_Double vreg_77;
System_Double vreg_78;
System_Int32 vreg_80;
System_Int32 vreg_81;
System_Double vreg_83;
System_Double vreg_85;

local_9 = _this->pairs.get();
local_10 = 0;
vreg_59 = local_9->Length;
vreg_60 = (int)vreg_59;
goto label_10D;
label_12:
local_0 = ((*local_9)[local_10]).get();
local_1 = local_0->bi.get();
local_2 = local_0->bj.get();
vreg_5 = local_1->x;
vreg_6 = local_2->x;
local_3 = vreg_5-vreg_6;
vreg_8 = local_1->y;
vreg_9 = local_2->y;
local_4 = vreg_8-vreg_9;
vreg_11 = local_1->z;
vreg_12 = local_2->z;
local_5 = vreg_11-vreg_12;
vreg_14 = local_3*local_3;
vreg_15 = local_4*local_4;
vreg_16 = vreg_14+vreg_15;
vreg_17 = local_5*local_5;
local_6 = vreg_16+vreg_17;
vreg_19 = System_Math_Sqrt(local_6);
vreg_20 = local_6*vreg_19;
local_7 = dt/vreg_20;
vreg_23 = local_1->vx;
vreg_83 = local_2->mass;
vreg_25 = local_3*vreg_83;
vreg_26 = vreg_25*local_7;
vreg_27 = vreg_23-vreg_26;
local_1->vx = vreg_27;
vreg_29 = local_2->vx;
vreg_85 = local_1->mass;
vreg_31 = local_3*vreg_85;
vreg_32 = vreg_31*local_7;
vreg_33 = vreg_29+vreg_32;
local_2->vx = vreg_33;
vreg_35 = local_1->vy;
vreg_37 = local_4*vreg_83;
vreg_38 = vreg_37*local_7;
vreg_39 = vreg_35-vreg_38;
local_1->vy = vreg_39;
vreg_41 = local_2->vy;
vreg_43 = local_4*vreg_85;
vreg_44 = vreg_43*local_7;
vreg_45 = vreg_41+vreg_44;
local_2->vy = vreg_45;
vreg_47 = local_1->vz;
vreg_49 = local_5*vreg_83;
vreg_50 = vreg_49*local_7;
vreg_51 = vreg_47-vreg_50;
local_1->vz = vreg_51;
vreg_53 = local_2->vz;
vreg_55 = local_5*vreg_85;
vreg_56 = vreg_55*local_7;
vreg_57 = vreg_53+vreg_56;
local_2->vz = vreg_57;
local_10 = local_10+1;
label_10D:
local_11 = (local_10 < vreg_60)?1:0;
if(local_11) goto label_12;
local_12 = _this->bodies.get();
local_10 = 0;
vreg_80 = local_12->Length;
vreg_81 = (int)vreg_80;
goto label_180;
label_12C:
local_8 = ((*local_12)[local_10]).get();
vreg_65 = local_8->x;
vreg_66 = local_8->vx;
vreg_67 = dt*vreg_66;
vreg_68 = vreg_65+vreg_67;
local_8->x = vreg_68;
vreg_70 = local_8->y;
vreg_71 = local_8->vy;
vreg_72 = dt*vreg_71;
vreg_73 = vreg_70+vreg_72;
local_8->y = vreg_73;
vreg_75 = local_8->z;
vreg_76 = local_8->vz;
vreg_77 = dt*vreg_76;
vreg_78 = vreg_75+vreg_77;
local_8->z = vreg_78;
local_10 = local_10+1;
label_180:
local_11 = (local_10 < vreg_81)?1:0;
if(local_11) goto label_12C;
return;
}


System_Void _Body_ctor()

{

return;
}


System_Void _Pair_ctor()

{

return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
_NBody_Main();
return 0;
}
void mapLibs() {
}

void RuntimeHelpersBuildConstantTable() {
}

void buildStringTable() {
_AddJumpAndLength(0, 5);
} // buildStringTable
const wchar_t _stringTable[6] = {
78, 66, 111, 100, 121, 0 /* "NBody" */
}; // _stringTable 

