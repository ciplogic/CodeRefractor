#include "sloth.h"
#include <functional>
struct _NBody; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct CodeRefactor_OpenRuntime_CrString; 
struct _NBodySystem; 
struct _Body; 
struct _Pair; 
struct CodeRefactor_OpenRuntime_CrMath; 
struct System_Object {
int _typeId;
};
struct System_ValueType : public System_Object {
};
struct _NBody : public System_Object {
};
struct System_Console : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
};
struct _NBodySystem : public System_Object {
 std::shared_ptr< Array < std::shared_ptr<_Body> > > bodies;
 std::shared_ptr< Array < std::shared_ptr<_Pair> > > pairs;
};
struct _Body : public System_Object {
 System_Double x;
 System_Double y;
 System_Double z;
 System_Double vx;
 System_Double vy;
 System_Double vz;
 System_Double mass;
};
struct _Pair : public System_Object {
 std::shared_ptr<_Body> bi;
 std::shared_ptr<_Body> bj;
};
struct System_Math : public System_Object {
};

System_Void _NBody_Main();

System_Void _NBodySystem_ctor(const std::shared_ptr<_NBodySystem>& _this);

System_Double _NBodySystem_Energy(const std::shared_ptr<_NBodySystem>& _this);

System_Void _NBodySystem_Advance(const std::shared_ptr<_NBodySystem>& _this, System_Double dt);

System_Void _Body_ctor();

System_Void _Pair_ctor();

#include "runtime_base.hpp"
// --- Begin definition of virtual method tables ---
System_Void setupTypeTable();

// --- End of definition of virtual method tables ---

#include "stdio.h"
System_Void CodeRefactor_OpenRuntime_CrConsole_WriteLine(std::shared_ptr<System_String> value)
{ printf("%ls\n", value.get()->Text->Items); }
System_Void CodeRefactor_OpenRuntime_CrConsole_WriteLine(System_Double value)
{ printf("%lf\n", value); }
#include "math.h"
System_Double CodeRefactor_OpenRuntime_CrMath_Sqrt(System_Double d)
{ return sqrt(d); }
///--- PInvoke code --- 
///---Begin closure code --- 
System_Void _NBody_Main()

{
std::shared_ptr<_NBodySystem> local_1;
System_Int32 local_2;
System_Boolean local_3;
std::shared_ptr<_NBodySystem> vreg_1;
System_Double vreg_3;
System_Double vreg_6;

CodeRefactor_OpenRuntime_CrConsole_WriteLine();
vreg_1 = std::make_shared<_NBodySystem >();
vreg_1->_typeId = 3;
_NBodySystem_ctor();
local_1 = vreg_1;
vreg_3 = _NBodySystem_Energy();
CodeRefactor_OpenRuntime_CrConsole_WriteLine();
local_2 = 0;
goto label_3C;
label_28:
_NBodySystem_Advance();
local_2 = local_2+1;
label_3C:
local_3 = (local_2 < 5000000)?1:0;
if(local_3) goto label_28;
vreg_6 = _NBodySystem_Energy();
CodeRefactor_OpenRuntime_CrConsole_WriteLine();
return;
}


System_Void _NBodySystem_ctor(const std::shared_ptr<_NBodySystem>& _this)

{
System_Int32 local_0;
System_Int32 local_1;
System_Int32 local_2;
System_Double local_3;
System_Double local_4;
System_Double local_5;
System_Boolean local_14;
std::shared_ptr< Array < std::shared_ptr<_Body> > > local_15;
System_Int32 local_16;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_1;
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
std::shared_ptr<_Pair> vreg_27;
std::shared_ptr<_Body> vreg_29;
std::shared_ptr<_Pair> vreg_31;
std::shared_ptr<_Body> vreg_33;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_36;
System_Int32 vreg_37;
System_Int32 vreg_38;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_41;
System_Int32 vreg_42;
System_Int32 vreg_43;
System_Int32 vreg_44;
std::shared_ptr<_Body> vreg_47;
System_Double vreg_48;
System_Double vreg_50;
System_Double vreg_52;
System_Double vreg_54;
System_Double vreg_56;
System_Double vreg_58;
System_Int32 vreg_61;
System_Int32 vreg_62;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_64;
std::shared_ptr<_Body> vreg_65;
System_Double vreg_66;
System_Double vreg_67;
System_Double vreg_68;
System_Double vreg_69;
System_Double vreg_70;
System_Double vreg_71;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_72;
std::shared_ptr< Array < std::shared_ptr<_Pair> > > vreg_74;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_75;
System_Double vreg_77;
System_Int32 vreg_78;
System_Int32 vreg_79;

vreg_1 = std::make_shared< Array <std::shared_ptr<_Body>> >(5); 
vreg_2 = std::make_shared<_Body >();
vreg_2->_typeId = 5;
_Body_ctor();
vreg_2->mass = 39.4784176043574;
(*vreg_1)[0] = vreg_2; 
vreg_4 = std::make_shared<_Body >();
vreg_4->_typeId = 5;
_Body_ctor();
vreg_4->x = 4.84143144246472;
vreg_4->y = -1.16032004402743;
vreg_4->z = -0.103622044471123;
vreg_4->vx = 0.606326392995832;
vreg_4->vy = 2.81198684491626;
vreg_4->vz = -0.0252183616598876;
vreg_4->mass = 0.0376936748703895;
(*vreg_1)[1] = vreg_4; 
vreg_6 = std::make_shared<_Body >();
vreg_6->_typeId = 5;
_Body_ctor();
vreg_6->x = 8.34336671824458;
vreg_6->y = 4.1247985641243;
vreg_6->z = -0.403523417114321;
vreg_6->vx = -1.01077434617879;
vreg_6->vy = 1.82566237123041;
vreg_6->vz = 0.00841576137658415;
vreg_6->mass = 0.0112863261319688;
(*vreg_1)[2] = vreg_6; 
vreg_8 = std::make_shared<_Body >();
vreg_8->_typeId = 5;
_Body_ctor();
vreg_8->x = 12.8943695621391;
vreg_8->y = -15.1111514016986;
vreg_8->z = -0.223307578892656;
vreg_8->vx = 1.08279100644154;
vreg_8->vy = 0.868713018169608;
vreg_8->vz = -0.0108326374013636;
vreg_8->mass = 0.00172372405705971;
(*vreg_1)[3] = vreg_8; 
vreg_10 = std::make_shared<_Body >();
vreg_10->_typeId = 5;
_Body_ctor();
vreg_10->x = 15.3796971148509;
vreg_10->y = -25.919314609988;
vreg_10->z = 0.179258772950371;
vreg_10->vx = 0.979090732243898;
vreg_10->vy = 0.594698998647676;
vreg_10->vz = -0.0347559555040781;
vreg_10->mass = 0.00203368686992463;
(*vreg_1)[4] = vreg_10; 
_this->bodies = vreg_1;
vreg_72 = _this->bodies;
vreg_78 = vreg_72->Length;
vreg_79 = (int)vreg_78;
vreg_18 = vreg_79-1;
vreg_19 = vreg_79*vreg_18;
vreg_20 = vreg_19/2;
vreg_21 = std::make_shared< Array <std::shared_ptr<_Pair>> >(vreg_20); 
_this->pairs = vreg_21;
local_0 = 0;
local_1 = 0;
goto label_2AB;
label_24F:
local_2 = local_1+1;
goto label_296;
label_255:
vreg_74 = _this->pairs;
vreg_24 = std::make_shared<_Pair >();
vreg_24->_typeId = 6;
_Pair_ctor();
(*vreg_74)[local_0] = vreg_24; 
vreg_27 = (*vreg_74)[local_0];
vreg_75 = _this->bodies;
vreg_29 = (*vreg_75)[local_1];
vreg_27->bi = vreg_29;
vreg_31 = (*vreg_74)[local_0];
vreg_33 = (*vreg_75)[local_2];
vreg_31->bj = vreg_33;
local_0 = local_0+1;
local_2 = local_2+1;
label_296:
vreg_36 = _this->bodies;
vreg_37 = vreg_36->Length;
vreg_38 = (int)vreg_37;
local_14 = (local_2 < vreg_38)?1:0;
if(local_14) goto label_255;
local_1 = local_1+1;
label_2AB:
vreg_41 = _this->bodies;
vreg_42 = vreg_41->Length;
vreg_43 = (int)vreg_42;
vreg_44 = vreg_43-1;
local_14 = (local_1 < vreg_44)?1:0;
if(local_14) goto label_24F;
local_3 = 0;
local_4 = 0;
local_5 = 0;
local_15 = _this->bodies;
local_16 = 0;
goto label_335;
label_2EC:
vreg_47 = (*local_15)[local_16];
vreg_48 = vreg_47->vx;
vreg_77 = vreg_47->mass;
vreg_50 = vreg_48*vreg_77;
local_3 = local_3+vreg_50;
vreg_52 = vreg_47->vy;
vreg_54 = vreg_52*vreg_77;
local_4 = local_4+vreg_54;
vreg_56 = vreg_47->vz;
vreg_58 = vreg_56*vreg_77;
local_5 = local_5+vreg_58;
local_16 = local_16+1;
label_335:
vreg_61 = local_15->Length;
vreg_62 = (int)vreg_61;
local_14 = (local_16 < vreg_62)?1:0;
if(local_14) goto label_2EC;
vreg_64 = _this->bodies;
vreg_65 = (*vreg_64)[0];
vreg_66 = -local_3;
vreg_67 = vreg_66/39.4784176043574;
vreg_65->vx = vreg_67;
vreg_68 = -local_4;
vreg_69 = vreg_68/39.4784176043574;
vreg_65->vy = vreg_69;
vreg_70 = -local_5;
vreg_71 = vreg_70/39.4784176043574;
vreg_65->vz = vreg_71;
return;
}


System_Double _NBodySystem_Energy(const std::shared_ptr<_NBodySystem>& _this)

{
System_Double local_0;
System_Int32 local_1;
_Body * local_2;
System_Int32 local_3;
System_Double local_8;
System_Boolean local_9;
Array < std::shared_ptr<_Body> >  * vreg_1;
_Body * vreg_2;
System_Double vreg_3;
System_Double vreg_4;
System_Double vreg_7;
System_Double vreg_10;
System_Double vreg_11;
System_Double vreg_14;
System_Double vreg_15;
System_Double vreg_16;
Array < std::shared_ptr<_Body> >  * vreg_19;
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
Array < std::shared_ptr<_Body> >  * vreg_42;
System_Int32 vreg_43;
System_Int32 vreg_44;
Array < std::shared_ptr<_Body> >  * vreg_47;
System_Int32 vreg_48;
System_Int32 vreg_49;
System_Double vreg_50;
System_Double vreg_51;
System_Double vreg_52;

local_0 = 0;
local_1 = 0;
goto label_DD;
label_12:
vreg_1 = _this->bodies.get();
vreg_2 = ((*vreg_1)[local_1]).get();
local_2 = vreg_2;
vreg_3 = vreg_2->mass;
vreg_4 = 0.5*vreg_3;
vreg_50 = vreg_2->vx;
vreg_7 = vreg_50*vreg_50;
vreg_51 = vreg_2->vy;
vreg_10 = vreg_51*vreg_51;
vreg_11 = vreg_7+vreg_10;
vreg_52 = vreg_2->vz;
vreg_14 = vreg_52*vreg_52;
vreg_15 = vreg_11+vreg_14;
vreg_16 = vreg_4*vreg_15;
local_0 = local_0+vreg_16;
local_3 = local_1+1;
goto label_C7;
label_5F:
vreg_19 = _this->bodies.get();
vreg_20 = ((*vreg_19)[local_3]).get();
vreg_21 = local_2->x;
vreg_22 = vreg_20->x;
vreg_23 = vreg_21-vreg_22;
vreg_24 = local_2->y;
vreg_25 = vreg_20->y;
vreg_26 = vreg_24-vreg_25;
vreg_27 = local_2->z;
vreg_28 = vreg_20->z;
vreg_29 = vreg_27-vreg_28;
vreg_30 = local_2->mass;
vreg_31 = vreg_20->mass;
vreg_32 = vreg_30*vreg_31;
vreg_33 = vreg_23*vreg_23;
vreg_34 = vreg_26*vreg_26;
vreg_35 = vreg_33+vreg_34;
vreg_36 = vreg_29*vreg_29;
vreg_37 = vreg_35+vreg_36;
vreg_38 = CodeRefactor_OpenRuntime_CrMath_Sqrt();
vreg_39 = vreg_32/vreg_38;
local_0 = local_0-vreg_39;
local_3 = local_3+1;
label_C7:
vreg_42 = _this->bodies.get();
vreg_43 = vreg_42->Length;
vreg_44 = (int)vreg_43;
local_9 = (local_3 < vreg_44)?1:0;
if(local_9) goto label_5F;
local_1 = local_1+1;
label_DD:
vreg_47 = _this->bodies.get();
vreg_48 = vreg_47->Length;
vreg_49 = (int)vreg_48;
local_9 = (local_1 < vreg_49)?1:0;
if(local_9) goto label_12;
local_8 = local_0;
label_F6:
return local_8;
}


System_Void _NBodySystem_Advance(const std::shared_ptr<_NBodySystem>& _this, System_Double dt)

{
_Pair * local_0;
Array < std::shared_ptr<_Pair> >  * local_9;
System_Int32 local_10;
System_Boolean local_11;
Array < std::shared_ptr<_Body> >  * local_12;
_Pair * vreg_2;
_Body * vreg_3;
_Body * vreg_4;
System_Double vreg_5;
System_Double vreg_6;
System_Double vreg_7;
System_Double vreg_8;
System_Double vreg_9;
System_Double vreg_10;
System_Double vreg_11;
System_Double vreg_12;
System_Double vreg_13;
System_Double vreg_14;
System_Double vreg_15;
System_Double vreg_16;
System_Double vreg_17;
System_Double vreg_18;
System_Double vreg_19;
System_Double vreg_20;
System_Double vreg_21;
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
_Body * vreg_63;
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
goto label_10D;
label_12:
vreg_2 = ((*local_9)[local_10]).get();
vreg_3 = vreg_2->bi.get();
vreg_4 = vreg_2->bj.get();
vreg_5 = vreg_3->x;
vreg_6 = vreg_4->x;
vreg_7 = vreg_5-vreg_6;
vreg_8 = vreg_3->y;
vreg_9 = vreg_4->y;
vreg_10 = vreg_8-vreg_9;
vreg_11 = vreg_3->z;
vreg_12 = vreg_4->z;
vreg_13 = vreg_11-vreg_12;
vreg_14 = vreg_7*vreg_7;
vreg_15 = vreg_10*vreg_10;
vreg_16 = vreg_14+vreg_15;
vreg_17 = vreg_13*vreg_13;
vreg_18 = vreg_16+vreg_17;
vreg_19 = CodeRefactor_OpenRuntime_CrMath_Sqrt();
vreg_20 = vreg_18*vreg_19;
vreg_21 = dt/vreg_20;
vreg_23 = vreg_3->vx;
vreg_83 = vreg_4->mass;
vreg_25 = vreg_7*vreg_83;
vreg_26 = vreg_25*vreg_21;
vreg_27 = vreg_23-vreg_26;
vreg_3->vx = vreg_27;
vreg_29 = vreg_4->vx;
vreg_85 = vreg_3->mass;
vreg_31 = vreg_7*vreg_85;
vreg_32 = vreg_31*vreg_21;
vreg_33 = vreg_29+vreg_32;
vreg_4->vx = vreg_33;
vreg_35 = vreg_3->vy;
vreg_37 = vreg_10*vreg_83;
vreg_38 = vreg_37*vreg_21;
vreg_39 = vreg_35-vreg_38;
vreg_3->vy = vreg_39;
vreg_41 = vreg_4->vy;
vreg_43 = vreg_10*vreg_85;
vreg_44 = vreg_43*vreg_21;
vreg_45 = vreg_41+vreg_44;
vreg_4->vy = vreg_45;
vreg_47 = vreg_3->vz;
vreg_49 = vreg_13*vreg_83;
vreg_50 = vreg_49*vreg_21;
vreg_51 = vreg_47-vreg_50;
vreg_3->vz = vreg_51;
vreg_53 = vreg_4->vz;
vreg_55 = vreg_13*vreg_85;
vreg_56 = vreg_55*vreg_21;
vreg_57 = vreg_53+vreg_56;
vreg_4->vz = vreg_57;
local_10 = local_10+1;
label_10D:
vreg_59 = local_9->Length;
vreg_60 = (int)vreg_59;
local_11 = (local_10 < vreg_60)?1:0;
if(local_11) goto label_12;
local_12 = _this->bodies.get();
local_10 = 0;
goto label_180;
label_12C:
vreg_63 = ((*local_12)[local_10]).get();
vreg_65 = vreg_63->x;
vreg_66 = vreg_63->vx;
vreg_67 = dt*vreg_66;
vreg_68 = vreg_65+vreg_67;
vreg_63->x = vreg_68;
vreg_70 = vreg_63->y;
vreg_71 = vreg_63->vy;
vreg_72 = dt*vreg_71;
vreg_73 = vreg_70+vreg_72;
vreg_63->y = vreg_73;
vreg_75 = vreg_63->z;
vreg_76 = vreg_63->vz;
vreg_77 = dt*vreg_76;
vreg_78 = vreg_75+vreg_77;
vreg_63->z = vreg_78;
local_10 = local_10+1;
label_180:
vreg_80 = local_12->Length;
vreg_81 = (int)vreg_80;
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
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

