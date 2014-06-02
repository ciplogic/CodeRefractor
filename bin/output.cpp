#include "sloth.h"
struct System_Object; 
struct _Pair; 
struct System_Math; 
struct _NBody; 
struct System_ValueType; 
struct System_Console; 
struct System_String; 
struct _Body; 
struct _NBodySystem; 
struct System_Object {
int _typeId;
};
struct _Pair : public System_Object {
 std::shared_ptr<_Body> bi;
 std::shared_ptr<_Body> bj;
};
struct System_Math : public System_Object {
};
struct _NBody : public System_Object {
};
struct System_ValueType : public System_Object {
};
struct System_Console : public System_Object {
};
struct System_String : public System_Object {
 std::shared_ptr< Array < System_Char > > Text;
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
struct _NBodySystem : public System_Object {
 std::shared_ptr< Array < std::shared_ptr<_Body> > > bodies;
 std::shared_ptr< Array < std::shared_ptr<_Pair> > > pairs;
};

System_Void _NBody_Main();

System_Void _NBodySystem_ctor(const std::shared_ptr<_NBodySystem>& _this);

System_Void _NBodySystem_Advance(const std::shared_ptr<_NBodySystem>& _this, System_Double dt);

System_Double _NBodySystem_Energy(const std::shared_ptr<_NBodySystem>& _this);

System_Void _Body_ctor(const std::shared_ptr<_Body>& _this);

System_Void _NBodySystem_CalculatePairs(const std::shared_ptr<_NBodySystem>& _this);

System_Void _NBodySystem_SetupDefaultParis(const std::shared_ptr<_NBodySystem>& _this);

System_Void _Pair_ctor(const std::shared_ptr<_Pair>& _this);

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
std::shared_ptr<_NBodySystem> vreg_1;
System_Double vreg_3;

System_Console_WriteLine(_str(0));
vreg_1 = std::make_shared<_NBodySystem >();
vreg_1->_typeId = 2;
_NBodySystem_ctor(vreg_1);
_NBodySystem_Advance(vreg_1, 3);
vreg_3 = _NBodySystem_Energy(vreg_1);
System_Console_WriteLine(vreg_3);
return;
}


System_Void _NBodySystem_ctor(const std::shared_ptr<_NBodySystem>& _this)

{
std::shared_ptr<_Body> local_0;
std::shared_ptr<_Body> local_1;
std::shared_ptr<_Body> local_2;
std::shared_ptr<_Body> local_3;
std::shared_ptr<_Body> local_4;
std::shared_ptr< Array < std::shared_ptr<_Body> > > local_5;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_1;
std::shared_ptr<_Body> vreg_2;
std::shared_ptr<_Body> vreg_3;
std::shared_ptr<_Body> vreg_4;
std::shared_ptr<_Body> vreg_5;
std::shared_ptr<_Body> vreg_6;
std::shared_ptr<_Body> vreg_7;
std::shared_ptr<_Body> vreg_8;
std::shared_ptr<_Body> vreg_9;
std::shared_ptr<_Body> vreg_10;
std::shared_ptr<_Body> vreg_11;

vreg_1 = std::make_shared< Array <std::shared_ptr<_Body>> >(5); 
local_5 = vreg_1;
vreg_2 = std::make_shared<_Body >();
vreg_2->_typeId = 3;
_Body_ctor(vreg_2);
vreg_3 = vreg_2;
local_0 = vreg_3;
local_0->mass = 39.4784176043574;
(*local_5)[0] = local_0; 
vreg_4 = std::make_shared<_Body >();
vreg_4->_typeId = 3;
_Body_ctor(vreg_4);
vreg_5 = vreg_4;
local_1 = vreg_5;
local_1->x = 4.84143144246472;
local_1->y = -1.16032004402743;
local_1->z = -0.103622044471123;
local_1->vx = 0.606326392995832;
local_1->vy = 2.81198684491626;
local_1->vz = -0.0252183616598876;
local_1->mass = 0.0376936748703895;
(*local_5)[1] = local_1; 
vreg_6 = std::make_shared<_Body >();
vreg_6->_typeId = 3;
_Body_ctor(vreg_6);
vreg_7 = vreg_6;
local_2 = vreg_7;
local_2->x = 8.34336671824458;
local_2->y = 4.1247985641243;
local_2->z = -0.403523417114321;
local_2->vx = -1.01077434617879;
local_2->vy = 1.82566237123041;
local_2->vz = 0.00841576137658415;
local_2->mass = 0.0112863261319688;
(*local_5)[2] = local_2; 
vreg_8 = std::make_shared<_Body >();
vreg_8->_typeId = 3;
_Body_ctor(vreg_8);
vreg_9 = vreg_8;
local_3 = vreg_9;
local_3->x = 12.8943695621391;
local_3->y = -15.1111514016986;
local_3->z = -0.223307578892656;
local_3->vx = 1.08279100644154;
local_3->vy = 0.868713018169608;
local_3->vz = -0.0108326374013636;
local_3->mass = 0.00172372405705971;
(*local_5)[3] = local_3; 
vreg_10 = std::make_shared<_Body >();
vreg_10->_typeId = 3;
_Body_ctor(vreg_10);
vreg_11 = vreg_10;
local_4 = vreg_11;
local_4->x = 15.3796971148509;
local_4->y = -25.919314609988;
local_4->z = 0.179258772950371;
local_4->vx = 0.979090732243898;
local_4->vy = 0.594698998647676;
local_4->vz = -0.0347559555040781;
local_4->mass = 0.00203368686992463;
(*local_5)[4] = local_4; 
_this->bodies = local_5;
_NBodySystem_CalculatePairs(_this);
return;
}


System_Void _NBodySystem_Advance(const std::shared_ptr<_NBodySystem>& _this, System_Double dt)

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
Array < std::shared_ptr<_Pair> > * vreg_1;
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
_Body * vreg_22;
System_Double vreg_23;
System_Double vreg_24;
System_Double vreg_25;
System_Double vreg_26;
System_Double vreg_27;
_Body * vreg_28;
System_Double vreg_29;
System_Double vreg_30;
System_Double vreg_31;
System_Double vreg_32;
System_Double vreg_33;
_Body * vreg_34;
System_Double vreg_35;
System_Double vreg_36;
System_Double vreg_37;
System_Double vreg_38;
System_Double vreg_39;
_Body * vreg_40;
System_Double vreg_41;
System_Double vreg_42;
System_Double vreg_43;
System_Double vreg_44;
System_Double vreg_45;
_Body * vreg_46;
System_Double vreg_47;
System_Double vreg_48;
System_Double vreg_49;
System_Double vreg_50;
System_Double vreg_51;
_Body * vreg_52;
System_Double vreg_53;
System_Double vreg_54;
System_Double vreg_55;
System_Double vreg_56;
System_Double vreg_57;
System_Int32 vreg_58;
System_Int32 vreg_59;
System_Int32 vreg_60;
System_Int32 vreg_61;
Array < std::shared_ptr<_Body> > * vreg_62;
_Body * vreg_63;
_Body * vreg_64;
System_Double vreg_65;
System_Double vreg_66;
System_Double vreg_67;
System_Double vreg_68;
_Body * vreg_69;
System_Double vreg_70;
System_Double vreg_71;
System_Double vreg_72;
System_Double vreg_73;
_Body * vreg_74;
System_Double vreg_75;
System_Double vreg_76;
System_Double vreg_77;
System_Double vreg_78;
System_Int32 vreg_79;
System_Int32 vreg_80;
System_Int32 vreg_81;
System_Int32 vreg_82;

vreg_1 = _this->pairs.get();
local_9 = vreg_1;
local_10 = 0;
goto label_10D;
label_12:
vreg_2 = ((*local_9)[local_10]).get();
local_0 = vreg_2;
vreg_3 = local_0->bi.get();
local_1 = vreg_3;
vreg_4 = local_0->bj.get();
local_2 = vreg_4;
vreg_5 = local_1->x;
vreg_6 = local_2->x;
vreg_7 = vreg_5-vreg_6;
local_3 = vreg_7;
vreg_8 = local_1->y;
vreg_9 = local_2->y;
vreg_10 = vreg_8-vreg_9;
local_4 = vreg_10;
vreg_11 = local_1->z;
vreg_12 = local_2->z;
vreg_13 = vreg_11-vreg_12;
local_5 = vreg_13;
vreg_14 = local_3*local_3;
vreg_15 = local_4*local_4;
vreg_16 = vreg_14+vreg_15;
vreg_17 = local_5*local_5;
vreg_18 = vreg_16+vreg_17;
local_6 = vreg_18;
vreg_19 = System_Math_Sqrt(local_6);
vreg_20 = local_6*vreg_19;
vreg_21 = dt/vreg_20;
local_7 = vreg_21;
vreg_22 = local_1;
vreg_23 = vreg_22->vx;
vreg_24 = local_2->mass;
vreg_25 = local_3*vreg_24;
vreg_26 = vreg_25*local_7;
vreg_27 = vreg_23-vreg_26;
local_1->vx = vreg_27;
vreg_28 = local_2;
vreg_29 = vreg_28->vx;
vreg_30 = local_1->mass;
vreg_31 = local_3*vreg_30;
vreg_32 = vreg_31*local_7;
vreg_33 = vreg_29+vreg_32;
local_2->vx = vreg_33;
vreg_34 = local_1;
vreg_35 = vreg_34->vy;
vreg_36 = local_2->mass;
vreg_37 = local_4*vreg_36;
vreg_38 = vreg_37*local_7;
vreg_39 = vreg_35-vreg_38;
local_1->vy = vreg_39;
vreg_40 = local_2;
vreg_41 = vreg_40->vy;
vreg_42 = local_1->mass;
vreg_43 = local_4*vreg_42;
vreg_44 = vreg_43*local_7;
vreg_45 = vreg_41+vreg_44;
local_2->vy = vreg_45;
vreg_46 = local_1;
vreg_47 = vreg_46->vz;
vreg_48 = local_2->mass;
vreg_49 = local_5*vreg_48;
vreg_50 = vreg_49*local_7;
vreg_51 = vreg_47-vreg_50;
local_1->vz = vreg_51;
vreg_52 = local_2;
vreg_53 = vreg_52->vz;
vreg_54 = local_1->mass;
vreg_55 = local_5*vreg_54;
vreg_56 = vreg_55*local_7;
vreg_57 = vreg_53+vreg_56;
local_2->vz = vreg_57;
vreg_58 = local_10+1;
local_10 = vreg_58;
label_10D:
vreg_59 = local_9->Length;
vreg_60 = (int)vreg_59;
vreg_61 = (local_10 < vreg_60)?1:0;
local_11 = vreg_61;
if(local_11) goto label_12;
vreg_62 = _this->bodies.get();
local_12 = vreg_62;
local_10 = 0;
goto label_180;
label_12C:
vreg_63 = ((*local_12)[local_10]).get();
local_8 = vreg_63;
vreg_64 = local_8;
vreg_65 = vreg_64->x;
vreg_66 = local_8->vx;
vreg_67 = dt*vreg_66;
vreg_68 = vreg_65+vreg_67;
local_8->x = vreg_68;
vreg_69 = local_8;
vreg_70 = vreg_69->y;
vreg_71 = local_8->vy;
vreg_72 = dt*vreg_71;
vreg_73 = vreg_70+vreg_72;
local_8->y = vreg_73;
vreg_74 = local_8;
vreg_75 = vreg_74->z;
vreg_76 = local_8->vz;
vreg_77 = dt*vreg_76;
vreg_78 = vreg_75+vreg_77;
local_8->z = vreg_78;
vreg_79 = local_10+1;
local_10 = vreg_79;
label_180:
vreg_80 = local_12->Length;
vreg_81 = (int)vreg_80;
vreg_82 = (local_10 < vreg_81)?1:0;
local_11 = vreg_82;
if(local_11) goto label_12C;
return;
}


System_Double _NBodySystem_Energy(const std::shared_ptr<_NBodySystem>& _this)

{
System_Double local_0;
System_Int32 local_1;
_Body * local_2;
System_Int32 local_3;
_Body * local_4;
System_Double local_5;
System_Double local_6;
System_Double local_7;
System_Double local_8;
System_Boolean local_9;
Array < std::shared_ptr<_Body> > * vreg_1;
_Body * vreg_2;
System_Double vreg_3;
System_Double vreg_4;
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
System_Int32 vreg_18;
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
System_Double vreg_40;
System_Int32 vreg_41;
Array < std::shared_ptr<_Body> > * vreg_42;
System_Int32 vreg_43;
System_Int32 vreg_44;
System_Int32 vreg_45;
System_Int32 vreg_46;
Array < std::shared_ptr<_Body> > * vreg_47;
System_Int32 vreg_48;
System_Int32 vreg_49;
System_Int32 vreg_50;

local_0 = 0;
local_1 = 0;
goto label_DD;
label_12:
vreg_1 = _this->bodies.get();
vreg_2 = ((*vreg_1)[local_1]).get();
local_2 = vreg_2;
vreg_3 = local_2->mass;
vreg_4 = 0.5*vreg_3;
vreg_5 = local_2->vx;
vreg_6 = local_2->vx;
vreg_7 = vreg_5*vreg_6;
vreg_8 = local_2->vy;
vreg_9 = local_2->vy;
vreg_10 = vreg_8*vreg_9;
vreg_11 = vreg_7+vreg_10;
vreg_12 = local_2->vz;
vreg_13 = local_2->vz;
vreg_14 = vreg_12*vreg_13;
vreg_15 = vreg_11+vreg_14;
vreg_16 = vreg_4*vreg_15;
vreg_17 = local_0+vreg_16;
local_0 = vreg_17;
vreg_18 = local_1+1;
local_3 = vreg_18;
goto label_C7;
label_5F:
vreg_19 = _this->bodies.get();
vreg_20 = ((*vreg_19)[local_3]).get();
local_4 = vreg_20;
vreg_21 = local_2->x;
vreg_22 = local_4->x;
vreg_23 = vreg_21-vreg_22;
local_5 = vreg_23;
vreg_24 = local_2->y;
vreg_25 = local_4->y;
vreg_26 = vreg_24-vreg_25;
local_6 = vreg_26;
vreg_27 = local_2->z;
vreg_28 = local_4->z;
vreg_29 = vreg_27-vreg_28;
local_7 = vreg_29;
vreg_30 = local_2->mass;
vreg_31 = local_4->mass;
vreg_32 = vreg_30*vreg_31;
vreg_33 = local_5*local_5;
vreg_34 = local_6*local_6;
vreg_35 = vreg_33+vreg_34;
vreg_36 = local_7*local_7;
vreg_37 = vreg_35+vreg_36;
vreg_38 = System_Math_Sqrt(vreg_37);
vreg_39 = vreg_32/vreg_38;
vreg_40 = local_0-vreg_39;
local_0 = vreg_40;
vreg_41 = local_3+1;
local_3 = vreg_41;
label_C7:
vreg_42 = _this->bodies.get();
vreg_43 = vreg_42->Length;
vreg_44 = (int)vreg_43;
vreg_45 = (local_3 < vreg_44)?1:0;
local_9 = vreg_45;
if(local_9) goto label_5F;
vreg_46 = local_1+1;
local_1 = vreg_46;
label_DD:
vreg_47 = _this->bodies.get();
vreg_48 = vreg_47->Length;
vreg_49 = (int)vreg_48;
vreg_50 = (local_1 < vreg_49)?1:0;
local_9 = vreg_50;
if(local_9) goto label_12;
local_8 = local_0;
goto label_F6;
label_F6:
return local_8;
}


System_Void _Body_ctor(const std::shared_ptr<_Body>& _this)

{

return;
}


System_Void _NBodySystem_CalculatePairs(const std::shared_ptr<_NBodySystem>& _this)

{
System_Double local_0;
System_Double local_1;
System_Double local_2;
_Body * local_3;
_Body * local_4;
Array < std::shared_ptr<_Body> > * local_5;
System_Int32 local_6;
System_Boolean local_7;
Array < std::shared_ptr<_Body> > * vreg_1;
_Body * vreg_2;
System_Double vreg_3;
System_Double vreg_4;
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
System_Int32 vreg_15;
System_Int32 vreg_16;
System_Int32 vreg_17;
System_Int32 vreg_18;
Array < std::shared_ptr<_Body> > * vreg_19;
_Body * vreg_20;
System_Double vreg_21;
System_Double vreg_22;
System_Double vreg_23;
System_Double vreg_24;
System_Double vreg_25;
System_Double vreg_26;

_NBodySystem_SetupDefaultParis(_this);
local_0 = 0;
local_1 = 0;
local_2 = 0;
vreg_1 = _this->bodies.get();
local_5 = vreg_1;
local_6 = 0;
goto label_72;
label_34:
vreg_2 = ((*local_5)[local_6]).get();
local_3 = vreg_2;
vreg_3 = local_3->vx;
vreg_4 = local_3->mass;
vreg_5 = vreg_3*vreg_4;
vreg_6 = local_0+vreg_5;
local_0 = vreg_6;
vreg_7 = local_3->vy;
vreg_8 = local_3->mass;
vreg_9 = vreg_7*vreg_8;
vreg_10 = local_1+vreg_9;
local_1 = vreg_10;
vreg_11 = local_3->vz;
vreg_12 = local_3->mass;
vreg_13 = vreg_11*vreg_12;
vreg_14 = local_2+vreg_13;
local_2 = vreg_14;
vreg_15 = local_6+1;
local_6 = vreg_15;
label_72:
vreg_16 = local_5->Length;
vreg_17 = (int)vreg_16;
vreg_18 = (local_6 < vreg_17)?1:0;
local_7 = vreg_18;
if(local_7) goto label_34;
vreg_19 = _this->bodies.get();
vreg_20 = ((*vreg_19)[0]).get();
local_4 = vreg_20;
vreg_21 = -local_0;
vreg_22 = vreg_21/39.4784176043574;
local_4->vx = vreg_22;
vreg_23 = -local_1;
vreg_24 = vreg_23/39.4784176043574;
local_4->vy = vreg_24;
vreg_25 = -local_2;
vreg_26 = vreg_25/39.4784176043574;
local_4->vz = vreg_26;
return;
}


System_Void _NBodySystem_SetupDefaultParis(const std::shared_ptr<_NBodySystem>& _this)

{
System_Int32 local_0;
System_Int32 local_1;
System_Int32 local_2;
System_Boolean local_3;
Array < std::shared_ptr<_Body> > * vreg_1;
System_Int32 vreg_2;
System_Int32 vreg_3;
Array < std::shared_ptr<_Body> > * vreg_4;
System_Int32 vreg_5;
System_Int32 vreg_6;
System_Int32 vreg_7;
System_Int32 vreg_8;
System_Int32 vreg_9;
std::shared_ptr< Array < std::shared_ptr<_Pair> > > vreg_10;
System_Int32 vreg_11;
Array < std::shared_ptr<_Pair> > * vreg_12;
std::shared_ptr<_Pair> vreg_13;
std::shared_ptr<_Pair> vreg_14;
Array < std::shared_ptr<_Pair> > * vreg_15;
_Pair * vreg_16;
Array < std::shared_ptr<_Body> > * vreg_17;
std::shared_ptr<_Body> vreg_18;
Array < std::shared_ptr<_Pair> > * vreg_19;
_Pair * vreg_20;
Array < std::shared_ptr<_Body> > * vreg_21;
std::shared_ptr<_Body> vreg_22;
System_Int32 vreg_23;
System_Int32 vreg_24;
Array < std::shared_ptr<_Body> > * vreg_25;
System_Int32 vreg_26;
System_Int32 vreg_27;
System_Int32 vreg_28;
System_Int32 vreg_29;
Array < std::shared_ptr<_Body> > * vreg_30;
System_Int32 vreg_31;
System_Int32 vreg_32;
System_Int32 vreg_33;
System_Int32 vreg_34;

vreg_1 = _this->bodies.get();
vreg_2 = vreg_1->Length;
vreg_3 = (int)vreg_2;
vreg_4 = _this->bodies.get();
vreg_5 = vreg_4->Length;
vreg_6 = (int)vreg_5;
vreg_7 = vreg_6-1;
vreg_8 = vreg_3*vreg_7;
vreg_9 = vreg_8/2;
vreg_10 = std::make_shared< Array <std::shared_ptr<_Pair>> >(vreg_9); 
_this->pairs = vreg_10;
local_0 = 0;
local_1 = 0;
goto label_81;
label_27:
vreg_11 = local_1+1;
local_2 = vreg_11;
goto label_6E;
label_2D:
vreg_12 = _this->pairs.get();
vreg_13 = std::make_shared<_Pair >();
vreg_13->_typeId = 5;
_Pair_ctor(vreg_13);
vreg_14 = vreg_13;
(*vreg_12)[local_0] = vreg_14; 
vreg_15 = _this->pairs.get();
vreg_16 = ((*vreg_15)[local_0]).get();
vreg_17 = _this->bodies.get();
vreg_18 = (*vreg_17)[local_1];
vreg_16->bi = vreg_18;
vreg_19 = _this->pairs.get();
vreg_20 = ((*vreg_19)[local_0]).get();
vreg_21 = _this->bodies.get();
vreg_22 = (*vreg_21)[local_2];
vreg_20->bj = vreg_22;
vreg_23 = local_0+1;
local_0 = vreg_23;
vreg_24 = local_2+1;
local_2 = vreg_24;
label_6E:
vreg_25 = _this->bodies.get();
vreg_26 = vreg_25->Length;
vreg_27 = (int)vreg_26;
vreg_28 = (local_2 < vreg_27)?1:0;
local_3 = vreg_28;
if(local_3) goto label_2D;
vreg_29 = local_1+1;
local_1 = vreg_29;
label_81:
vreg_30 = _this->bodies.get();
vreg_31 = vreg_30->Length;
vreg_32 = (int)vreg_31;
vreg_33 = vreg_32-1;
vreg_34 = (local_1 < vreg_33)?1:0;
local_3 = vreg_34;
if(local_3) goto label_27;
return;
}


System_Void _Pair_ctor(const std::shared_ptr<_Pair>& _this)

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
_AddJumpAndLength(0, 10);
} // buildStringTable
const wchar_t _stringTable[11] = {
83, 116, 97, 114, 116, 105, 110, 103, 58, 32, 0 /* "Starting: " */
}; // _stringTable 

