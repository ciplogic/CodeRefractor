#include "sloth.h"
namespace SimpleAdditions { struct NBody; }
namespace SimpleAdditions { struct NBodySystem; }
namespace System { struct Console; }
namespace SimpleAdditions { struct Body; }
namespace SimpleAdditions { struct Pair; }
namespace System { struct Math; }
namespace SimpleAdditions {
struct NBody {
}; }
namespace SimpleAdditions {
struct NBodySystem {
 std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > bodies;
 std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Pair> > > pairs;
 static System::Double Pi;
 static System::Double Solarmass;
 static System::Double DaysPeryear;
}; }
namespace System {
struct Console {
}; }
namespace SimpleAdditions {
struct Body {
 System::Double mass;
 System::Double x;
 System::Double y;
 System::Double z;
 System::Double vx;
 System::Double vy;
 System::Double vz;
}; }
namespace SimpleAdditions {
struct Pair {
 std::shared_ptr<SimpleAdditions::Body> bi;
 std::shared_ptr<SimpleAdditions::Body> bj;
}; }
namespace System {
struct Math {
}; }
System::Void SimpleAdditions_NBody__Main();

System::Void SimpleAdditions_NBodySystem__NBodySystem_ctor(const std::shared_ptr<SimpleAdditions::NBodySystem>& _this);

System::Double SimpleAdditions_NBodySystem__Energy(const std::shared_ptr<SimpleAdditions::NBodySystem>& _this);

System::Void System_Console__WriteLine(System::Double value);

System::Void SimpleAdditions_NBodySystem__Advance(const std::shared_ptr<SimpleAdditions::NBodySystem>& _this, System::Double dt);

System::Void SimpleAdditions_Body__Body_ctor(const std::shared_ptr<SimpleAdditions::Body>& _this);

System::Void SimpleAdditions_Pair__Pair_ctor(const std::shared_ptr<SimpleAdditions::Pair>& _this);

System::Double System_Math__Sqrt(System::Double d);

#include "runtime_base.partcpp"
#include "math.h"
System::Double System_Math__Sqrt(System::Double d)
{ return sqrt(d); }
#include "stdio.h"
System::Void System_Console__WriteLine(System::Double value)
{ printf("%lf\n", value); }
///---Begin closure code --- 
System::Void SimpleAdditions_NBody__Main()
{
System::Int32 local_3;
std::shared_ptr<SimpleAdditions::NBodySystem> vreg_5;
System::Double vreg_8;
System::Int32 vreg_18;
System::Double vreg_20;

vreg_5 = std::make_shared<SimpleAdditions::NBodySystem>();
SimpleAdditions_NBodySystem__NBodySystem_ctor(vreg_5);
vreg_8 = SimpleAdditions_NBodySystem__Energy(vreg_5);
System_Console__WriteLine(vreg_8);
local_3 = 0;
goto label_74;
label_54:
SimpleAdditions_NBodySystem__Advance(vreg_5, 0.01);
local_3 = local_3+1;
label_74:
vreg_18 = (local_3 < 5000000)?1:0;
if(vreg_18) goto label_54;
vreg_20 = SimpleAdditions_NBodySystem__Energy(vreg_5);
System_Console__WriteLine(vreg_20);
return;
}


System::Void SimpleAdditions_NBodySystem__NBodySystem_ctor(const std::shared_ptr<SimpleAdditions::NBodySystem>& _this)
{
System::Int32 local_0;
System::Int32 local_1;
System::Int32 local_2;
System::Double local_4;
System::Double local_5;
System::Double local_6;
System::Boolean local_15;
System::Int32 local_17;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_4;
std::shared_ptr<SimpleAdditions::Body> vreg_7;
std::shared_ptr<SimpleAdditions::Body> vreg_14;
std::shared_ptr<SimpleAdditions::Body> vreg_33;
std::shared_ptr<SimpleAdditions::Body> vreg_52;
std::shared_ptr<SimpleAdditions::Body> vreg_71;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_91;
System::Int32 vreg_92;
System::Int32 vreg_93;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_95;
System::Int32 vreg_96;
System::Int32 vreg_97;
System::Int32 vreg_99;
System::Int32 vreg_100;
System::Int32 vreg_102;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Pair> > > vreg_103;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Pair> > > vreg_110;
System::Int32 vreg_111;
std::shared_ptr<SimpleAdditions::Pair> vreg_115;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_119;
std::shared_ptr<SimpleAdditions::Body> vreg_121;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_124;
std::shared_ptr<SimpleAdditions::Body> vreg_126;
System::Int32 vreg_131;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_133;
System::Int32 vreg_134;
System::Int32 vreg_135;
System::Int32 vreg_141;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_143;
System::Int32 vreg_144;
System::Int32 vreg_145;
System::Int32 vreg_147;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_154;
std::shared_ptr<SimpleAdditions::Body> vreg_158;
System::Double vreg_159;
System::Double vreg_161;
System::Double vreg_163;
System::Double vreg_164;
System::Double vreg_166;
System::Double vreg_168;
System::Double vreg_170;
System::Double vreg_171;
System::Double vreg_173;
System::Double vreg_175;
System::Double vreg_177;
System::Double vreg_178;
System::Int32 vreg_183;
System::Int32 vreg_185;
System::Int32 vreg_186;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_190;
std::shared_ptr<SimpleAdditions::Body> vreg_192;
System::Double vreg_195;
System::Double vreg_197;
System::Double vreg_200;
System::Double vreg_202;
System::Double vreg_205;
System::Double vreg_207;

vreg_4 = std::make_shared< Array < std::shared_ptr<SimpleAdditions::Body> > >(5); 
vreg_7 = std::make_shared<SimpleAdditions::Body>();
SimpleAdditions_Body__Body_ctor(vreg_7);
vreg_7->mass = 39.4784176043574;
(*vreg_4)[0] = vreg_7; 
vreg_14 = std::make_shared<SimpleAdditions::Body>();
SimpleAdditions_Body__Body_ctor(vreg_14);
vreg_14->x = 4.84143144246472;
vreg_14->y = -1.16032004402743;
vreg_14->z = -0.103622044471123;
vreg_14->vx = 0.606326392995832;
vreg_14->vy = 2.81198684491626;
vreg_14->vz = -0.0252183616598876;
vreg_14->mass = 0.0376936748703895;
(*vreg_4)[1] = vreg_14; 
vreg_33 = std::make_shared<SimpleAdditions::Body>();
SimpleAdditions_Body__Body_ctor(vreg_33);
vreg_33->x = 8.34336671824458;
vreg_33->y = 4.1247985641243;
vreg_33->z = -0.403523417114321;
vreg_33->vx = -1.01077434617879;
vreg_33->vy = 1.82566237123041;
vreg_33->vz = 0.00841576137658415;
vreg_33->mass = 0.0112863261319688;
(*vreg_4)[2] = vreg_33; 
vreg_52 = std::make_shared<SimpleAdditions::Body>();
SimpleAdditions_Body__Body_ctor(vreg_52);
vreg_52->x = 12.8943695621391;
vreg_52->y = -15.1111514016986;
vreg_52->z = -0.223307578892656;
vreg_52->vx = 1.08279100644154;
vreg_52->vy = 0.868713018169608;
vreg_52->vz = -0.0108326374013636;
vreg_52->mass = 0.00172372405705971;
(*vreg_4)[3] = vreg_52; 
vreg_71 = std::make_shared<SimpleAdditions::Body>();
SimpleAdditions_Body__Body_ctor(vreg_71);
vreg_71->x = 15.3796971148509;
vreg_71->y = -25.919314609988;
vreg_71->z = 0.179258772950371;
vreg_71->vx = 0.979090732243898;
vreg_71->vy = 0.594698998647676;
vreg_71->vz = -0.0347559555040781;
vreg_71->mass = 0.00203368686992463;
(*vreg_4)[4] = vreg_71; 
_this->bodies = vreg_4;
vreg_91 = _this->bodies;
vreg_92 = vreg_91->Length;
vreg_93 = (int)vreg_92;
vreg_95 = _this->bodies;
vreg_96 = vreg_95->Length;
vreg_97 = (int)vreg_96;
vreg_99 = vreg_97-1;
vreg_100 = vreg_93*vreg_99;
vreg_102 = vreg_100/2;
vreg_103 = std::make_shared< Array < std::shared_ptr<SimpleAdditions::Pair> > >(vreg_102); 
_this->pairs = vreg_103;
local_0 = 0;
local_1 = 0;
goto label_669;
label_591:
local_2 = local_1+1;
goto label_648;
label_597:
vreg_110 = _this->pairs;
vreg_111 = local_0;
local_0 = local_0+1;
vreg_115 = std::make_shared<SimpleAdditions::Pair>();
SimpleAdditions_Pair__Pair_ctor(vreg_115);
vreg_119 = _this->bodies;
vreg_121 = (*vreg_119)[local_1];
vreg_115->bi = vreg_121;
vreg_124 = _this->bodies;
vreg_126 = (*vreg_124)[local_2];
vreg_115->bj = vreg_126;
(*vreg_110)[vreg_111] = vreg_115; 
local_2 = local_2+1;
label_648:
vreg_131 = local_2;
vreg_133 = _this->bodies;
vreg_134 = vreg_133->Length;
vreg_135 = (int)vreg_134;
local_15 = (vreg_131 < vreg_135)?1:0;
if(local_15) goto label_597;
local_1 = local_1+1;
label_669:
vreg_141 = local_1;
vreg_143 = _this->bodies;
vreg_144 = vreg_143->Length;
vreg_145 = (int)vreg_144;
vreg_147 = vreg_145-1;
local_15 = (vreg_141 < vreg_147)?1:0;
if(local_15) goto label_591;
local_4 = 0;
local_5 = 0;
local_6 = 0;
vreg_154 = _this->bodies;
local_17 = 0;
goto label_810;
label_735:
vreg_158 = (*vreg_154)[local_17];
vreg_159 = local_4;
vreg_161 = vreg_158->vx;
vreg_163 = vreg_158->mass;
vreg_164 = vreg_161*vreg_163;
local_4 = vreg_159+vreg_164;
vreg_166 = local_5;
vreg_168 = vreg_158->vy;
vreg_170 = vreg_158->mass;
vreg_171 = vreg_168*vreg_170;
local_5 = vreg_166+vreg_171;
vreg_173 = local_6;
vreg_175 = vreg_158->vz;
vreg_177 = vreg_158->mass;
vreg_178 = vreg_175*vreg_177;
local_6 = vreg_173+vreg_178;
local_17 = local_17+1;
label_810:
vreg_183 = local_17;
vreg_185 = vreg_154->Length;
vreg_186 = (int)vreg_185;
local_15 = (vreg_183 < vreg_186)?1:0;
if(local_15) goto label_735;
vreg_190 = _this->bodies;
vreg_192 = (*vreg_190)[0];
vreg_195 = -local_4;
vreg_197 = vreg_195/39.4784176043574;
vreg_192->vx = vreg_197;
vreg_200 = -local_5;
vreg_202 = vreg_200/39.4784176043574;
vreg_192->vy = vreg_202;
vreg_205 = -local_6;
vreg_207 = vreg_205/39.4784176043574;
vreg_192->vz = vreg_207;
return;
}


System::Double SimpleAdditions_NBodySystem__Energy(const std::shared_ptr<SimpleAdditions::NBodySystem>& _this)
{
System::Double local_0;
System::Int32 local_1;
System::Int32 local_3;
System::Double local_5;
System::Double local_6;
System::Double local_7;
System::Boolean local_9;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_4;
std::shared_ptr<SimpleAdditions::Body> vreg_6;
System::Double vreg_7;
System::Double vreg_10;
System::Double vreg_11;
System::Double vreg_13;
System::Double vreg_15;
System::Double vreg_16;
System::Double vreg_18;
System::Double vreg_20;
System::Double vreg_21;
System::Double vreg_22;
System::Double vreg_24;
System::Double vreg_26;
System::Double vreg_27;
System::Double vreg_28;
System::Double vreg_29;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_35;
std::shared_ptr<SimpleAdditions::Body> vreg_37;
System::Double vreg_39;
System::Double vreg_41;
System::Double vreg_44;
System::Double vreg_46;
System::Double vreg_49;
System::Double vreg_51;
System::Double vreg_53;
System::Double vreg_55;
System::Double vreg_57;
System::Double vreg_58;
System::Double vreg_61;
System::Double vreg_64;
System::Double vreg_65;
System::Double vreg_68;
System::Double vreg_69;
System::Double vreg_70;
System::Double vreg_71;
System::Int32 vreg_76;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_78;
System::Int32 vreg_79;
System::Int32 vreg_80;
System::Int32 vreg_86;
std::shared_ptr< Array < std::shared_ptr<SimpleAdditions::Body> > > vreg_88;
System::Int32 vreg_89;
System::Int32 vreg_90;

local_0 = 0;
local_1 = 0;
goto label_221;
label_18:
vreg_4 = _this->bodies;
vreg_6 = (*vreg_4)[local_1];
vreg_7 = local_0;
vreg_10 = vreg_6->mass;
vreg_11 = 0.5*vreg_10;
vreg_13 = vreg_6->vx;
vreg_15 = vreg_6->vx;
vreg_16 = vreg_13*vreg_15;
vreg_18 = vreg_6->vy;
vreg_20 = vreg_6->vy;
vreg_21 = vreg_18*vreg_20;
vreg_22 = vreg_16+vreg_21;
vreg_24 = vreg_6->vz;
vreg_26 = vreg_6->vz;
vreg_27 = vreg_24*vreg_26;
vreg_28 = vreg_22+vreg_27;
vreg_29 = vreg_11*vreg_28;
local_0 = vreg_7+vreg_29;
local_3 = local_1+1;
goto label_199;
label_95:
vreg_35 = _this->bodies;
vreg_37 = (*vreg_35)[local_3];
vreg_39 = vreg_6->x;
vreg_41 = vreg_37->x;
local_5 = vreg_39-vreg_41;
vreg_44 = vreg_6->y;
vreg_46 = vreg_37->y;
local_6 = vreg_44-vreg_46;
vreg_49 = vreg_6->z;
vreg_51 = vreg_37->z;
local_7 = vreg_49-vreg_51;
vreg_53 = local_0;
vreg_55 = vreg_6->mass;
vreg_57 = vreg_37->mass;
vreg_58 = vreg_55*vreg_57;
vreg_61 = local_5*local_5;
vreg_64 = local_6*local_6;
vreg_65 = vreg_61+vreg_64;
vreg_68 = local_7*local_7;
vreg_69 = vreg_65+vreg_68;
vreg_70 = System_Math__Sqrt(vreg_69);
vreg_71 = vreg_58/vreg_70;
local_0 = vreg_53-vreg_71;
local_3 = local_3+1;
label_199:
vreg_76 = local_3;
vreg_78 = _this->bodies;
vreg_79 = vreg_78->Length;
vreg_80 = (int)vreg_79;
local_9 = (vreg_76 < vreg_80)?1:0;
if(local_9) goto label_95;
local_1 = local_1+1;
label_221:
vreg_86 = local_1;
vreg_88 = _this->bodies;
vreg_89 = vreg_88->Length;
vreg_90 = (int)vreg_89;
local_9 = (vreg_86 < vreg_90)?1:0;
if(local_9) goto label_18;
return local_0;
}


System::Void SimpleAdditions_NBodySystem__Advance(const std::shared_ptr<SimpleAdditions::NBodySystem>& _this, System::Double dt)
{
System::Double local_3;
System::Double local_4;
System::Double local_5;
System::Double local_6;
System::Double local_7;
System::Int32 local_10;
System::Boolean local_11;
Array < std::shared_ptr<SimpleAdditions::Pair> > * vreg_2;
SimpleAdditions::Pair* vreg_6;
SimpleAdditions::Body* vreg_8;
SimpleAdditions::Body* vreg_10;
System::Double vreg_12;
System::Double vreg_14;
System::Double vreg_17;
System::Double vreg_19;
System::Double vreg_22;
System::Double vreg_24;
System::Double vreg_28;
System::Double vreg_31;
System::Double vreg_32;
System::Double vreg_35;
System::Double vreg_38;
System::Double vreg_40;
System::Double vreg_41;
System::Double vreg_45;
System::Double vreg_46;
System::Double vreg_48;
System::Double vreg_49;
System::Double vreg_51;
System::Double vreg_52;
System::Double vreg_55;
System::Double vreg_56;
System::Double vreg_58;
System::Double vreg_59;
System::Double vreg_61;
System::Double vreg_62;
System::Double vreg_65;
System::Double vreg_66;
System::Double vreg_68;
System::Double vreg_69;
System::Double vreg_71;
System::Double vreg_72;
System::Double vreg_75;
System::Double vreg_76;
System::Double vreg_78;
System::Double vreg_79;
System::Double vreg_81;
System::Double vreg_82;
System::Double vreg_85;
System::Double vreg_86;
System::Double vreg_88;
System::Double vreg_89;
System::Double vreg_91;
System::Double vreg_92;
System::Double vreg_95;
System::Double vreg_96;
System::Double vreg_98;
System::Double vreg_99;
System::Double vreg_101;
System::Double vreg_102;
System::Int32 vreg_106;
System::Int32 vreg_108;
System::Int32 vreg_109;
Array < std::shared_ptr<SimpleAdditions::Body> > * vreg_113;
SimpleAdditions::Body* vreg_117;
System::Double vreg_120;
System::Double vreg_123;
System::Double vreg_124;
System::Double vreg_125;
System::Double vreg_128;
System::Double vreg_131;
System::Double vreg_132;
System::Double vreg_133;
System::Double vreg_136;
System::Double vreg_139;
System::Double vreg_140;
System::Double vreg_141;
System::Int32 vreg_145;
System::Int32 vreg_147;
System::Int32 vreg_148;

vreg_2 = _this->pairs.get();
local_10 = 0;
goto label_269;
label_18:
vreg_6 = (*vreg_2)[local_10].get();
vreg_8 = vreg_6->bi.get();
vreg_10 = vreg_6->bj.get();
vreg_12 = vreg_8->x;
vreg_14 = vreg_10->x;
local_3 = vreg_12-vreg_14;
vreg_17 = vreg_8->y;
vreg_19 = vreg_10->y;
local_4 = vreg_17-vreg_19;
vreg_22 = vreg_8->z;
vreg_24 = vreg_10->z;
local_5 = vreg_22-vreg_24;
vreg_28 = local_3*local_3;
vreg_31 = local_4*local_4;
vreg_32 = vreg_28+vreg_31;
vreg_35 = local_5*local_5;
local_6 = vreg_32+vreg_35;
vreg_38 = local_6;
vreg_40 = System_Math__Sqrt(local_6);
vreg_41 = vreg_38*vreg_40;
local_7 = dt/vreg_41;
vreg_45 = vreg_8->vx;
vreg_46 = local_3;
vreg_48 = vreg_10->mass;
vreg_49 = vreg_46*vreg_48;
vreg_51 = vreg_49*local_7;
vreg_52 = vreg_45-vreg_51;
vreg_8->vx = vreg_52;
vreg_55 = vreg_10->vx;
vreg_56 = local_3;
vreg_58 = vreg_8->mass;
vreg_59 = vreg_56*vreg_58;
vreg_61 = vreg_59*local_7;
vreg_62 = vreg_55+vreg_61;
vreg_10->vx = vreg_62;
vreg_65 = vreg_8->vy;
vreg_66 = local_4;
vreg_68 = vreg_10->mass;
vreg_69 = vreg_66*vreg_68;
vreg_71 = vreg_69*local_7;
vreg_72 = vreg_65-vreg_71;
vreg_8->vy = vreg_72;
vreg_75 = vreg_10->vy;
vreg_76 = local_4;
vreg_78 = vreg_8->mass;
vreg_79 = vreg_76*vreg_78;
vreg_81 = vreg_79*local_7;
vreg_82 = vreg_75+vreg_81;
vreg_10->vy = vreg_82;
vreg_85 = vreg_8->vz;
vreg_86 = local_5;
vreg_88 = vreg_10->mass;
vreg_89 = vreg_86*vreg_88;
vreg_91 = vreg_89*local_7;
vreg_92 = vreg_85-vreg_91;
vreg_8->vz = vreg_92;
vreg_95 = vreg_10->vz;
vreg_96 = local_5;
vreg_98 = vreg_8->mass;
vreg_99 = vreg_96*vreg_98;
vreg_101 = vreg_99*local_7;
vreg_102 = vreg_95+vreg_101;
vreg_10->vz = vreg_102;
local_10 = local_10+1;
label_269:
vreg_106 = local_10;
vreg_108 = vreg_2->Length;
vreg_109 = (int)vreg_108;
local_11 = (vreg_106 < vreg_109)?1:0;
if(local_11) goto label_18;
vreg_113 = _this->bodies.get();
local_10 = 0;
goto label_384;
label_300:
vreg_117 = (*vreg_113)[local_10].get();
vreg_120 = vreg_117->x;
vreg_123 = vreg_117->vx;
vreg_124 = dt*vreg_123;
vreg_125 = vreg_120+vreg_124;
vreg_117->x = vreg_125;
vreg_128 = vreg_117->y;
vreg_131 = vreg_117->vy;
vreg_132 = dt*vreg_131;
vreg_133 = vreg_128+vreg_132;
vreg_117->y = vreg_133;
vreg_136 = vreg_117->z;
vreg_139 = vreg_117->vz;
vreg_140 = dt*vreg_139;
vreg_141 = vreg_136+vreg_140;
vreg_117->z = vreg_141;
local_10 = local_10+1;
label_384:
vreg_145 = local_10;
vreg_147 = vreg_113->Length;
vreg_148 = (int)vreg_147;
local_11 = (vreg_145 < vreg_148)?1:0;
if(local_11) goto label_300;
return;
}


System::Void SimpleAdditions_Body__Body_ctor(const std::shared_ptr<SimpleAdditions::Body>& _this)
{

return;
}


System::Void SimpleAdditions_Pair__Pair_ctor(const std::shared_ptr<SimpleAdditions::Pair>& _this)
{

return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
SimpleAdditions_NBody__Main();
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

