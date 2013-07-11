#include "sloth.h"
#include "runtime_base.partcpp"
#include "math.h"
System::Double System_Math__Sqrt(System::Double d)
{ return sqrt(d); }
#include "stdio.h"
void System_Console__WriteLine(System::Double value)
{ printf("%lf\n", value); }
namespace  {
struct NBody;
}
namespace  {
struct NBodySystem;
}
namespace  {
struct Body;
}
namespace  {
struct Pair;
}
namespace  {
struct NBody {
}; }
void _NBody__Main();
namespace  {
struct NBodySystem {
 std::shared_ptr< Array < std::shared_ptr<Body> > > bodies;
 std::shared_ptr< Array < std::shared_ptr<Pair> > > pairs;
}; }
void _NBodySystem__NBodySystem_ctor(const std::shared_ptr<NBodySystem>& _this);
System::Double _NBodySystem__Energy(const std::shared_ptr<NBodySystem>& _this);
void _NBodySystem__Advance(const std::shared_ptr<NBodySystem>& _this, System::Double dt);
namespace  {
struct Body {
 System::Double x;
 System::Double y;
 System::Double z;
 System::Double vx;
 System::Double vy;
 System::Double vz;
 System::Double mass;
}; }
namespace  {
struct Pair {
 std::shared_ptr<Body> bi;
 std::shared_ptr<Body> bj;
}; }
void _NBody__Main()
{
System::Int32 local_0;
std::shared_ptr<NBodySystem> local_1;
System::Int32 local_2;
System::Boolean local_3;
System::Double vreg_3;
System::Double vreg_7;

local_0 = 5000000;
local_1 = std::shared_ptr<NBodySystem>(new NBodySystem());
_NBodySystem__NBodySystem_ctor(local_1);
vreg_3 = _NBodySystem__Energy(local_1);
System_Console__WriteLine(vreg_3);
local_2 = 0;
goto label_49;
label_29:
_NBodySystem__Advance(local_1, 0.01);
local_2 = local_2+1;
label_49:
local_3 = (local_2 < local_0)?1:0;
if(local_3) goto label_29;
vreg_7 = _NBodySystem__Energy(local_1);
System_Console__WriteLine(vreg_7);
return;
}


void _NBodySystem__NBodySystem_ctor(const std::shared_ptr<NBodySystem>& _this)
{
System::Int32 local_0;
System::Int32 local_1;
System::Int32 local_2;
std::shared_ptr<Pair> local_3;
System::Double local_4;
System::Double local_5;
System::Double local_6;
std::shared_ptr<Body> local_7;
std::shared_ptr<Body> local_8;
std::shared_ptr<Body> local_9;
std::shared_ptr<Body> local_10;
std::shared_ptr<Body> local_11;
std::shared_ptr<Body> local_12;
std::shared_ptr<Body> local_13;
std::shared_ptr< Array < std::shared_ptr<Body> > > local_14;
System::Boolean local_15;
std::shared_ptr< Array < std::shared_ptr<Body> > > local_16;
System::Int32 local_17;
std::shared_ptr<NBodySystem> vreg_1;
System::Int32 vreg_2;
System::Int32 vreg_3;
System::Int32 vreg_4;
System::Int32 vreg_5;
System::Int32 vreg_7;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_8;
std::shared_ptr<NBodySystem> vreg_9;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_10;
System::Int32 vreg_11;
System::Int32 vreg_12;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_13;
System::Int32 vreg_14;
System::Int32 vreg_15;
System::Int32 vreg_16;
System::Int32 vreg_17;
System::Int32 vreg_18;
std::shared_ptr< Array < std::shared_ptr<Pair> > > vreg_19;
std::shared_ptr< Array < std::shared_ptr<Pair> > > vreg_20;
System::Int32 vreg_21;
std::shared_ptr<Pair> vreg_22;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_23;
std::shared_ptr<Body> vreg_24;
std::shared_ptr<Pair> vreg_25;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_26;
std::shared_ptr<Body> vreg_27;
System::Int32 vreg_28;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_29;
System::Int32 vreg_30;
System::Int32 vreg_31;
System::Int32 vreg_32;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_33;
System::Int32 vreg_34;
System::Int32 vreg_35;
System::Int32 vreg_36;
System::Double vreg_37;
System::Double vreg_38;
System::Double vreg_39;
System::Double vreg_40;
System::Double vreg_41;
System::Double vreg_42;
System::Double vreg_43;
System::Double vreg_44;
System::Double vreg_45;
System::Double vreg_46;
System::Double vreg_47;
System::Double vreg_48;
System::Int32 vreg_49;
System::Int32 vreg_50;
System::Int32 vreg_51;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_52;
std::shared_ptr<Body> vreg_53;
System::Double vreg_54;
System::Double vreg_55;
std::shared_ptr<Body> vreg_56;
System::Double vreg_57;
System::Double vreg_58;
std::shared_ptr<Body> vreg_59;
System::Double vreg_60;
System::Double vreg_61;


vreg_1 = _this;
local_14 = std::shared_ptr< Array < std::shared_ptr<Body> > > (new Array < std::shared_ptr<Body> >(5) ); 
vreg_2 = 0;
local_9 = std::shared_ptr<Body>(new Body());

local_9->mass = 39.4784176043574;
(*local_14)[vreg_2] = local_9; 
vreg_3 = 1;
local_10 = std::shared_ptr<Body>(new Body());

local_10->x = 4.84143144246472;
local_10->y = -1.16032004402743;
local_10->z = -0.103622044471123;
local_10->vx = 0.606326392995832;
local_10->vy = 2.81198684491626;
local_10->vz = -0.0252183616598876;
local_10->mass = 0.0376936748703895;
(*local_14)[vreg_3] = local_10; 
vreg_4 = 2;
local_11 = std::shared_ptr<Body>(new Body());

local_11->x = 8.34336671824458;
local_11->y = 4.1247985641243;
local_11->z = -0.403523417114321;
local_11->vx = -1.01077434617879;
local_11->vy = 1.82566237123041;
local_11->vz = 0.00841576137658415;
local_11->mass = 0.0112863261319688;
(*local_14)[vreg_4] = local_11; 
vreg_5 = 3;
local_12 = std::shared_ptr<Body>(new Body());

local_12->x = 12.8943695621391;
local_12->y = -15.1111514016986;
local_12->z = -0.223307578892656;
local_12->vx = 1.08279100644154;
local_12->vy = 0.868713018169608;
local_12->vz = -0.0108326374013636;
local_12->mass = 0.00172372405705971;
(*local_14)[vreg_5] = local_12; 
vreg_7 = 4;
local_13 = std::shared_ptr<Body>(new Body());

local_13->x = 15.3796971148509;
local_13->y = -25.919314609988;
local_13->z = 0.179258772950371;
local_13->vx = 0.979090732243898;
local_13->vy = 0.594698998647676;
local_13->vz = -0.0347559555040781;
local_13->mass = 0.00203368686992463;
(*local_14)[vreg_7] = local_13; 
vreg_8 = local_14;
vreg_1->bodies = vreg_8;
vreg_9 = _this;
vreg_10 = _this->bodies;
vreg_11 = vreg_10->Length;
vreg_12 = (int)vreg_11;
vreg_13 = _this->bodies;
vreg_14 = vreg_13->Length;
vreg_15 = (int)vreg_14;
vreg_16 = vreg_15-1;
vreg_17 = vreg_12*vreg_16;
vreg_18 = vreg_17/2;
vreg_19 = std::shared_ptr< Array < std::shared_ptr<Pair> > > (new Array < std::shared_ptr<Pair> >(vreg_18) ); 
vreg_9->pairs = vreg_19;
local_0 = 0;
local_1 = 0;
goto label_669;
label_591:
local_2 = local_1+1;
goto label_648;
label_597:
vreg_20 = _this->pairs;
vreg_21 = local_0;
local_0 = local_0+1;
local_3 = std::shared_ptr<Pair>(new Pair());

vreg_22 = local_3;
vreg_23 = _this->bodies;
vreg_24 = (*vreg_23)[local_1];
vreg_22->bi = vreg_24;
vreg_25 = local_3;
vreg_26 = _this->bodies;
vreg_27 = (*vreg_26)[local_2];
vreg_25->bj = vreg_27;
(*vreg_20)[vreg_21] = local_3; 
local_2 = local_2+1;
label_648:
vreg_28 = local_2;
vreg_29 = _this->bodies;
vreg_30 = vreg_29->Length;
vreg_31 = (int)vreg_30;
local_15 = (vreg_28 < vreg_31)?1:0;
if(local_15) goto label_597;
local_1 = local_1+1;
label_669:
vreg_32 = local_1;
vreg_33 = _this->bodies;
vreg_34 = vreg_33->Length;
vreg_35 = (int)vreg_34;
vreg_36 = vreg_35-1;
local_15 = (vreg_32 < vreg_36)?1:0;
if(local_15) goto label_591;
local_4 = 0;
local_5 = 0;
local_6 = 0;
local_16 = _this->bodies;
local_17 = 0;
goto label_810;
label_735:
local_7 = (*local_16)[local_17];
vreg_37 = local_4;
vreg_38 = local_7->vx;
vreg_39 = local_7->mass;
vreg_40 = vreg_38*vreg_39;
local_4 = vreg_37+vreg_40;
vreg_41 = local_5;
vreg_42 = local_7->vy;
vreg_43 = local_7->mass;
vreg_44 = vreg_42*vreg_43;
local_5 = vreg_41+vreg_44;
vreg_45 = local_6;
vreg_46 = local_7->vz;
vreg_47 = local_7->mass;
vreg_48 = vreg_46*vreg_47;
local_6 = vreg_45+vreg_48;
local_17 = local_17+1;
label_810:
vreg_49 = local_17;
vreg_50 = local_16->Length;
vreg_51 = (int)vreg_50;
local_15 = (vreg_49 < vreg_51)?1:0;
if(local_15) goto label_735;
vreg_52 = _this->bodies;
local_8 = (*vreg_52)[0];
vreg_53 = local_8;
vreg_54 = -local_4;
vreg_55 = vreg_54/39.4784176043574;
vreg_53->vx = vreg_55;
vreg_56 = local_8;
vreg_57 = -local_5;
vreg_58 = vreg_57/39.4784176043574;
vreg_56->vy = vreg_58;
vreg_59 = local_8;
vreg_60 = -local_6;
vreg_61 = vreg_60/39.4784176043574;
vreg_59->vz = vreg_61;
return;
}


System::Double _NBodySystem__Energy(const std::shared_ptr<NBodySystem>& _this)
{
System::Double local_0;
System::Int32 local_1;
std::shared_ptr<Body> local_2;
System::Int32 local_3;
std::shared_ptr<Body> local_4;
System::Double local_5;
System::Double local_6;
System::Double local_7;
System::Double local_8;
System::Boolean local_9;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_1;
System::Double vreg_2;
System::Double vreg_3;
System::Double vreg_4;
System::Double vreg_5;
System::Double vreg_6;
System::Double vreg_7;
System::Double vreg_8;
System::Double vreg_9;
System::Double vreg_10;
System::Double vreg_11;
System::Double vreg_12;
System::Double vreg_13;
System::Double vreg_14;
System::Double vreg_15;
System::Double vreg_16;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_17;
System::Double vreg_18;
System::Double vreg_19;
System::Double vreg_20;
System::Double vreg_21;
System::Double vreg_22;
System::Double vreg_23;
System::Double vreg_24;
System::Double vreg_25;
System::Double vreg_26;
System::Double vreg_27;
System::Double vreg_28;
System::Double vreg_29;
System::Double vreg_30;
System::Double vreg_31;
System::Double vreg_32;
System::Double vreg_33;
System::Double vreg_34;
System::Int32 vreg_35;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_36;
System::Int32 vreg_37;
System::Int32 vreg_38;
System::Int32 vreg_39;
std::shared_ptr< Array < std::shared_ptr<Body> > > vreg_40;
System::Int32 vreg_41;
System::Int32 vreg_42;
System::Double vreg_43;

local_0 = 0;
local_1 = 0;
goto label_221;
label_18:
vreg_1 = _this->bodies;
local_2 = (*vreg_1)[local_1];
vreg_2 = local_0;
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
local_0 = vreg_2+vreg_16;
local_3 = local_1+1;
goto label_199;
label_95:
vreg_17 = _this->bodies;
local_4 = (*vreg_17)[local_3];
vreg_18 = local_2->x;
vreg_19 = local_4->x;
local_5 = vreg_18-vreg_19;
vreg_20 = local_2->y;
vreg_21 = local_4->y;
local_6 = vreg_20-vreg_21;
vreg_22 = local_2->z;
vreg_23 = local_4->z;
local_7 = vreg_22-vreg_23;
vreg_24 = local_0;
vreg_25 = local_2->mass;
vreg_26 = local_4->mass;
vreg_27 = vreg_25*vreg_26;
vreg_28 = local_5*local_5;
vreg_29 = local_6*local_6;
vreg_30 = vreg_28+vreg_29;
vreg_31 = local_7*local_7;
vreg_32 = vreg_30+vreg_31;
vreg_33 = System_Math__Sqrt(vreg_32);
vreg_34 = vreg_27/vreg_33;
local_0 = vreg_24-vreg_34;
local_3 = local_3+1;
label_199:
vreg_35 = local_3;
vreg_36 = _this->bodies;
vreg_37 = vreg_36->Length;
vreg_38 = (int)vreg_37;
local_9 = (vreg_35 < vreg_38)?1:0;
if(local_9) goto label_95;
local_1 = local_1+1;
label_221:
vreg_39 = local_1;
vreg_40 = _this->bodies;
vreg_41 = vreg_40->Length;
vreg_42 = (int)vreg_41;
local_9 = (vreg_39 < vreg_42)?1:0;
if(local_9) goto label_18;
local_8 = local_0;
vreg_43 = local_8;
return vreg_43;
}


void _NBodySystem__Advance(const std::shared_ptr<NBodySystem>& _this, System::Double dt)
{
std::shared_ptr<Pair> local_0;
std::shared_ptr<Body> local_1;
std::shared_ptr<Body> local_2;
System::Double local_3;
System::Double local_4;
System::Double local_5;
System::Double local_6;
System::Double local_7;
std::shared_ptr<Body> local_8;
std::shared_ptr< Array < std::shared_ptr<Pair> > > local_9;
System::Int32 local_10;
System::Boolean local_11;
std::shared_ptr< Array < std::shared_ptr<Body> > > local_12;
System::Double vreg_1;
System::Double vreg_2;
System::Double vreg_3;
System::Double vreg_4;
System::Double vreg_5;
System::Double vreg_6;
System::Double vreg_7;
System::Double vreg_8;
System::Double vreg_9;
System::Double vreg_10;
System::Double vreg_11;
System::Double vreg_12;
System::Double vreg_13;
System::Double vreg_14;
System::Double vreg_15;
System::Double vreg_16;
System::Double vreg_17;
System::Double vreg_18;
System::Double vreg_19;
System::Double vreg_20;
System::Double vreg_21;
System::Double vreg_22;
System::Double vreg_23;
System::Double vreg_24;
System::Double vreg_25;
System::Double vreg_26;
System::Double vreg_27;
System::Double vreg_28;
System::Double vreg_29;
System::Double vreg_30;
System::Double vreg_31;
System::Double vreg_32;
System::Double vreg_33;
System::Double vreg_34;
System::Double vreg_35;
System::Double vreg_36;
System::Double vreg_37;
System::Double vreg_38;
System::Double vreg_39;
System::Double vreg_40;
System::Double vreg_41;
System::Double vreg_42;
System::Double vreg_43;
System::Double vreg_44;
System::Double vreg_45;
System::Double vreg_46;
System::Double vreg_47;
System::Double vreg_48;
System::Double vreg_49;
System::Double vreg_50;
System::Int32 vreg_51;
System::Int32 vreg_52;
System::Int32 vreg_53;
System::Double vreg_54;
System::Double vreg_55;
System::Double vreg_56;
System::Double vreg_57;
System::Double vreg_58;
System::Double vreg_59;
System::Double vreg_60;
System::Double vreg_61;
System::Double vreg_62;
System::Double vreg_63;
System::Double vreg_65;
System::Double vreg_66;
System::Double vreg_67;
System::Double vreg_68;
System::Double vreg_69;
System::Int32 vreg_70;
System::Int32 vreg_71;
System::Int32 vreg_72;

local_9 = _this->pairs;
local_10 = 0;
goto label_269;
label_18:
local_0 = (*local_9)[local_10];
local_1 = local_0->bi;
local_2 = local_0->bj;
vreg_1 = local_1->x;
vreg_2 = local_2->x;
local_3 = vreg_1-vreg_2;
vreg_3 = local_1->y;
vreg_4 = local_2->y;
local_4 = vreg_3-vreg_4;
vreg_5 = local_1->z;
vreg_6 = local_2->z;
local_5 = vreg_5-vreg_6;
vreg_7 = local_3*local_3;
vreg_8 = local_4*local_4;
vreg_9 = vreg_7+vreg_8;
vreg_10 = local_5*local_5;
local_6 = vreg_9+vreg_10;
vreg_11 = dt;
vreg_12 = local_6;
vreg_13 = System_Math__Sqrt(local_6);
vreg_14 = vreg_12*vreg_13;
local_7 = vreg_11/vreg_14;
vreg_15 = local_1->vx;
vreg_16 = local_3;
vreg_17 = local_2->mass;
vreg_18 = vreg_16*vreg_17;
vreg_19 = vreg_18*local_7;
vreg_20 = vreg_15-vreg_19;
local_1->vx = vreg_20;
vreg_21 = local_2->vx;
vreg_22 = local_3;
vreg_23 = local_1->mass;
vreg_24 = vreg_22*vreg_23;
vreg_25 = vreg_24*local_7;
vreg_26 = vreg_21+vreg_25;
local_2->vx = vreg_26;
vreg_27 = local_1->vy;
vreg_28 = local_4;
vreg_29 = local_2->mass;
vreg_30 = vreg_28*vreg_29;
vreg_31 = vreg_30*local_7;
vreg_32 = vreg_27-vreg_31;
local_1->vy = vreg_32;
vreg_33 = local_2->vy;
vreg_34 = local_4;
vreg_35 = local_1->mass;
vreg_36 = vreg_34*vreg_35;
vreg_37 = vreg_36*local_7;
vreg_38 = vreg_33+vreg_37;
local_2->vy = vreg_38;
vreg_39 = local_1->vz;
vreg_40 = local_5;
vreg_41 = local_2->mass;
vreg_42 = vreg_40*vreg_41;
vreg_43 = vreg_42*local_7;
vreg_44 = vreg_39-vreg_43;
local_1->vz = vreg_44;
vreg_45 = local_2->vz;
vreg_46 = local_5;
vreg_47 = local_1->mass;
vreg_48 = vreg_46*vreg_47;
vreg_49 = vreg_48*local_7;
vreg_50 = vreg_45+vreg_49;
local_2->vz = vreg_50;
local_10 = local_10+1;
label_269:
vreg_51 = local_10;
vreg_52 = local_9->Length;
vreg_53 = (int)vreg_52;
local_11 = (vreg_51 < vreg_53)?1:0;
if(local_11) goto label_18;
local_12 = _this->bodies;
local_10 = 0;
goto label_384;
label_300:
local_8 = (*local_12)[local_10];
vreg_54 = local_8->x;
vreg_55 = dt;
vreg_56 = local_8->vx;
vreg_57 = vreg_55*vreg_56;
vreg_58 = vreg_54+vreg_57;
local_8->x = vreg_58;
vreg_59 = local_8->y;
vreg_60 = dt;
vreg_61 = local_8->vy;
vreg_62 = vreg_60*vreg_61;
vreg_63 = vreg_59+vreg_62;
local_8->y = vreg_63;
vreg_65 = local_8->z;
vreg_66 = dt;
vreg_67 = local_8->vz;
vreg_68 = vreg_66*vreg_67;
vreg_69 = vreg_65+vreg_68;
local_8->z = vreg_69;
local_10 = local_10+1;
label_384:
vreg_70 = local_10;
vreg_71 = local_12->Length;
vreg_72 = (int)vreg_71;
local_11 = (vreg_70 < vreg_72)?1:0;
if(local_11) goto label_300;
return;
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

