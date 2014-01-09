#include "sloth.h"
struct System_ValueType; 
struct System_Array; 
struct CodeRefactor_OpenRuntime_CrString; 
struct System_Object; 
struct CodeRefactor_OpenRuntime_CrStringBuilder; 
struct _Body; 
struct _Pair; 
struct _NBodySystem; 
struct _NBody; 
struct CodeRefactor_OpenRuntime_CrConsole; 
struct CodeRefactor_OpenRuntime_CrMath; 
struct System_ValueType {
};
struct System_Array {
};
struct CodeRefactor_OpenRuntime_CrString {
 std::shared_ptr< Array < System_Char > > Text;
};
struct System_Object {
};
struct CodeRefactor_OpenRuntime_CrStringBuilder {
 std::shared_ptr< Array < System_Char > > _data;
 System_Int32 _writtenLength;
};
struct _Body {
 System_Double x;
 System_Double y;
 System_Double z;
 System_Double vx;
 System_Double vy;
 System_Double vz;
 System_Double mass;
};
struct _Pair {
 std::shared_ptr<_Body> bi;
 std::shared_ptr<_Body> bj;
};
struct _NBodySystem {
 std::shared_ptr< Array < std::shared_ptr<_Body> > > bodies;
 std::shared_ptr< Array < std::shared_ptr<_Pair> > > pairs;
};
struct _NBody {
};
struct CodeRefactor_OpenRuntime_CrConsole {
};
struct CodeRefactor_OpenRuntime_CrMath {
};

System_Void _NBody_Main();

System_Void _NBodySystem_ctor(_NBodySystem * _this);

System_Double _NBodySystem_Energy(_NBodySystem * _this);

System_Void _NBodySystem_Advance(_NBodySystem * _this, System_Double  dt);

System_Void _NBodySystem_AdvancePairs(_NBodySystem * _this, System_Double  dt);

System_Void _NBodySystem_AdvanceBodies(_NBodySystem * _this, System_Double  dt);

#include "runtime_base.partcpp"
#include "stdio.h"
System_Void System_Console_WriteLine(System_Double value)
{ printf("%lf\n", value); }
#include "math.h"
System_Double System_Math_Sqrt(System_Double d)
{ return sqrt(d); }
///---Begin closure code --- 
System_Void _NBody_Main()
{
_NBodySystem * local_1;
System_Int32 local_2;
System_Double vreg_5;
System_Int32 vreg_11;
System_Boolean vreg_15;
System_Double vreg_17;

_NBodySystem  vreg_2;
_NBodySystem_ctor(&vreg_2);
local_1 = &vreg_2;
vreg_5 = _NBodySystem_Energy(&vreg_2);
System_Console_WriteLine(vreg_5);
local_2 = 0;
goto label_49;
label_29:
_NBodySystem_Advance(local_1, 0.01);
vreg_11 = local_2+1;
local_2 = vreg_11;
label_49:
vreg_15 = (local_2 < 500000)?1:0;
if(vreg_15) goto label_29;
vreg_17 = _NBodySystem_Energy(local_1);
System_Console_WriteLine(vreg_17);
return;
}


System_Void _NBodySystem_ctor(_NBodySystem * _this)
{
System_Int32 local_0;
System_Int32 local_1;
System_Int32 local_2;
System_Double local_4;
System_Double local_5;
System_Double local_6;
Array < std::shared_ptr<_Body> > * local_16;
System_Int32 local_17;
std::shared_ptr< Array < std::shared_ptr<_Body> > > vreg_4;
std::shared_ptr<_Body> vreg_7;
std::shared_ptr<_Body> vreg_14;
std::shared_ptr<_Body> vreg_33;
std::shared_ptr<_Body> vreg_52;
std::shared_ptr<_Body> vreg_71;
System_Int32 vreg_99;
System_Int32 vreg_100;
System_Int32 vreg_102;
std::shared_ptr< Array < std::shared_ptr<_Pair> > > vreg_103;
System_Int32 vreg_108;
Array < std::shared_ptr<_Pair> > * vreg_110;
System_Int32 vreg_111;
System_Int32 vreg_114;
std::shared_ptr<_Pair> vreg_115;
std::shared_ptr<_Body> vreg_121;
std::shared_ptr<_Body> vreg_126;
System_Int32 vreg_130;
System_Int32 vreg_135;
System_Boolean vreg_137;
System_Int32 vreg_140;
System_Int32 vreg_147;
System_Boolean vreg_149;
Array < std::shared_ptr<_Body> > * vreg_154;
_Body * vreg_158;
System_Double vreg_161;
System_Double vreg_164;
System_Double vreg_165;
System_Double vreg_168;
System_Double vreg_171;
System_Double vreg_172;
System_Double vreg_175;
System_Double vreg_178;
System_Double vreg_179;
System_Int32 vreg_182;
System_Int32 vreg_185;
System_Int32 vreg_186;
System_Boolean vreg_188;
Array < std::shared_ptr<_Body> > * vreg_190;
_Body * vreg_192;
System_Double vreg_195;
System_Double vreg_197;
System_Double vreg_200;
System_Double vreg_202;
System_Double vreg_205;
System_Double vreg_207;
Array < std::shared_ptr<_Body> > * vreg_211;
System_Double vreg_213;
System_Int32 vreg_218;
System_Int32 vreg_219;
Array < std::shared_ptr<_Body> > * vreg_220;

vreg_4 = std::make_shared< Array <std::shared_ptr<_Body>> >(5); 
vreg_7 = std::make_shared<_Body >();
vreg_7->mass = 39.4784176043574;
(*vreg_4)[0] = vreg_7; 
vreg_14 = std::make_shared<_Body >();
vreg_14->x = 4.84143144246472;
vreg_14->y = -1.16032004402743;
vreg_14->z = -0.103622044471123;
vreg_14->vx = 0.606326392995832;
vreg_14->vy = 2.81198684491626;
vreg_14->vz = -0.0252183616598876;
vreg_14->mass = 0.0376936748703895;
(*vreg_4)[1] = vreg_14; 
vreg_33 = std::make_shared<_Body >();
vreg_33->x = 8.34336671824458;
vreg_33->y = 4.1247985641243;
vreg_33->z = -0.403523417114321;
vreg_33->vx = -1.01077434617879;
vreg_33->vy = 1.82566237123041;
vreg_33->vz = 0.00841576137658415;
vreg_33->mass = 0.0112863261319688;
(*vreg_4)[2] = vreg_33; 
vreg_52 = std::make_shared<_Body >();
vreg_52->x = 12.8943695621391;
vreg_52->y = -15.1111514016986;
vreg_52->z = -0.223307578892656;
vreg_52->vx = 1.08279100644154;
vreg_52->vy = 0.868713018169608;
vreg_52->vz = -0.0108326374013636;
vreg_52->mass = 0.00172372405705971;
(*vreg_4)[3] = vreg_52; 
vreg_71 = std::make_shared<_Body >();
vreg_71->x = 15.3796971148509;
vreg_71->y = -25.919314609988;
vreg_71->z = 0.179258772950371;
vreg_71->vx = 0.979090732243898;
vreg_71->vy = 0.594698998647676;
vreg_71->vz = -0.0347559555040781;
vreg_71->mass = 0.00203368686992463;
(*vreg_4)[4] = vreg_71; 
_this->bodies = vreg_4;
vreg_220 = _this->bodies.get();
vreg_218 = vreg_220->Length;
vreg_219 = (int)vreg_218;
vreg_99 = vreg_219-1;
vreg_100 = vreg_219*vreg_99;
vreg_102 = vreg_100/2;
vreg_103 = std::make_shared< Array <std::shared_ptr<_Pair>> >(vreg_102); 
_this->pairs = vreg_103;
local_0 = 0;
local_1 = 0;
vreg_211 = vreg_220;
vreg_110 = _this->pairs.get();
vreg_135 = vreg_219;
vreg_147 = vreg_219-1;
goto label_669;
label_591:
local_2 = local_1+1;
//local_2 = vreg_108;
goto label_648;
label_597:
vreg_111 = local_0;
local_0 = local_0+1;
//local_0 = vreg_114;
vreg_115 = std::make_shared<_Pair >();
vreg_121 = (*vreg_211)[local_1];
vreg_115->bi = vreg_121;
vreg_126 = (*vreg_211)[local_2];
vreg_115->bj = vreg_126;
(*vreg_110)[vreg_111] = vreg_115; 
local_2 = local_2+1;
//local_2 = vreg_130;
label_648:
vreg_137 = (local_2 < vreg_135)?1:0;
if(vreg_137) goto label_597;
vreg_140 = local_1+1;
local_1 = vreg_140;
label_669:
vreg_149 = (local_1 < vreg_147)?1:0;
if(vreg_149) goto label_591;
local_4 = 0;
local_5 = 0;
local_6 = 0;
vreg_154 = _this->bodies.get();
local_16 = vreg_154;
local_17 = 0;
vreg_185 = vreg_154->Length;
vreg_186 = (int)vreg_185;
goto label_810;
label_735:
vreg_158 = ((*local_16)[local_17]).get();
vreg_161 = vreg_158->vx;
vreg_213 = vreg_158->mass;
vreg_164 = vreg_161*vreg_213;
local_4 = local_4+vreg_164;
//local_4 = vreg_165;
vreg_168 = vreg_158->vy;
vreg_171 = vreg_168*vreg_213;
local_5 = local_5+vreg_171;
//local_5 = vreg_172;
vreg_175 = vreg_158->vz;
vreg_178 = vreg_175*vreg_213;
local_6 = local_6+vreg_178;
//local_6 = vreg_179;
local_17 = local_17+1;
//local_17 = vreg_182;
label_810:
vreg_188 = (local_17 < vreg_186)?1:0;
if(vreg_188) goto label_735;
vreg_190 = _this->bodies.get();
vreg_192 = ((*vreg_190)[0]).get();
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


System_Double _NBodySystem_Energy(_NBodySystem * _this)
{
System_Double local_0;
System_Int32 local_1;
System_Int32 local_3;
System_Double local_5;
System_Double local_6;
System_Double local_7;
Array < std::shared_ptr<_Body> > * vreg_4;
_Body * vreg_6;
System_Double vreg_11;
System_Double vreg_16;
System_Double vreg_21;
System_Double vreg_22;
System_Double vreg_27;
System_Double vreg_28;
System_Double vreg_29;
System_Double vreg_30;
System_Int32 vreg_33;
Array < std::shared_ptr<_Body> > * vreg_35;
_Body * vreg_37;
System_Double vreg_39;
System_Double vreg_41;
System_Double vreg_44;
System_Double vreg_46;
System_Double vreg_49;
System_Double vreg_51;
System_Double vreg_55;
System_Double vreg_57;
System_Double vreg_58;
System_Double vreg_61;
System_Double vreg_64;
System_Double vreg_65;
System_Double vreg_68;
System_Double vreg_69;
System_Double vreg_70;
System_Double vreg_71;
System_Double vreg_72;
System_Int32 vreg_75;
System_Int32 vreg_80;
System_Boolean vreg_82;
System_Int32 vreg_85;
System_Int32 vreg_90;
System_Boolean vreg_92;
System_Double vreg_93;
System_Double vreg_94;
System_Double vreg_95;
System_Int32 vreg_97;
System_Int32 vreg_98;
Array < std::shared_ptr<_Body> > * vreg_100;
System_Double vreg_101;

local_0 = 0;
local_1 = 0;
vreg_100 = _this->bodies.get();
vreg_35 = vreg_100;
vreg_4 = vreg_100;
vreg_97 = vreg_100->Length;
vreg_98 = (int)vreg_97;
vreg_90 = vreg_98;
vreg_80 = vreg_98;
goto label_221;
label_18:
vreg_6 = ((*vreg_4)[local_1]).get();
vreg_101 = vreg_6->mass;
vreg_11 = 0.5*vreg_101;
vreg_93 = vreg_6->vx;
vreg_16 = vreg_93*vreg_93;
vreg_94 = vreg_6->vy;
vreg_21 = vreg_94*vreg_94;
vreg_22 = vreg_16+vreg_21;
vreg_95 = vreg_6->vz;
vreg_27 = vreg_95*vreg_95;
vreg_28 = vreg_22+vreg_27;
vreg_29 = vreg_11*vreg_28;
vreg_30 = local_0+vreg_29;
local_0 = vreg_30;
vreg_33 = local_1+1;
local_3 = vreg_33;
vreg_55 = vreg_101;
vreg_49 = vreg_6->z;
vreg_44 = vreg_6->y;
vreg_39 = vreg_6->x;
goto label_199;
label_95:
vreg_37 = ((*vreg_35)[local_3]).get();
vreg_41 = vreg_37->x;
local_5 = vreg_39-vreg_41;
vreg_46 = vreg_37->y;
local_6 = vreg_44-vreg_46;
vreg_51 = vreg_37->z;
local_7 = vreg_49-vreg_51;
vreg_57 = vreg_37->mass;
vreg_58 = vreg_55*vreg_57;
vreg_61 = local_5*local_5;
vreg_64 = local_6*local_6;
vreg_65 = vreg_61+vreg_64;
vreg_68 = local_7*local_7;
vreg_69 = vreg_65+vreg_68;
vreg_70 = System_Math_Sqrt(vreg_69);
vreg_71 = vreg_58/vreg_70;
vreg_72 = local_0-vreg_71;
local_0 = vreg_72;
vreg_75 = local_3+1;
local_3 = vreg_75;
label_199:
vreg_82 = (local_3 < vreg_80)?1:0;
if(vreg_82) goto label_95;
vreg_85 = local_1+1;
local_1 = vreg_85;
label_221:
vreg_92 = (local_1 < vreg_90)?1:0;
if(vreg_92) goto label_18;
return local_0;
}


System_Void _NBodySystem_Advance(_NBodySystem * _this, System_Double  dt)
{

_NBodySystem_AdvancePairs(_this, dt);
_NBodySystem_AdvanceBodies(_this, dt);
return;
}


System_Void _NBodySystem_AdvancePairs(_NBodySystem * _this, System_Double  dt)
{
_Pair * local_0;
System_Double local_3;
System_Double local_4;
System_Double local_5;
System_Double local_6;
System_Double local_7;
Array < std::shared_ptr<_Pair> > * local_8;
System_Int32 local_9;
Array < std::shared_ptr<_Pair> > * vreg_2;
_Pair * vreg_6;
_Body * vreg_8;
_Body * vreg_10;
System_Double vreg_12;
System_Double vreg_14;
System_Double vreg_17;
System_Double vreg_19;
System_Double vreg_22;
System_Double vreg_24;
System_Double vreg_28;
System_Double vreg_31;
System_Double vreg_32;
System_Double vreg_35;
System_Double vreg_40;
System_Double vreg_41;
System_Double vreg_45;
System_Double vreg_49;
System_Double vreg_51;
System_Double vreg_52;
System_Double vreg_55;
System_Double vreg_59;
System_Double vreg_61;
System_Double vreg_62;
System_Double vreg_65;
System_Double vreg_69;
System_Double vreg_71;
System_Double vreg_72;
System_Double vreg_75;
System_Double vreg_79;
System_Double vreg_81;
System_Double vreg_82;
System_Double vreg_85;
System_Double vreg_89;
System_Double vreg_91;
System_Double vreg_92;
System_Double vreg_95;
System_Double vreg_99;
System_Double vreg_101;
System_Double vreg_102;
System_Int32 vreg_105;
System_Int32 vreg_108;
System_Int32 vreg_109;
System_Boolean vreg_111;
System_Double vreg_113;
System_Double vreg_115;

vreg_2 = _this->pairs.get();
local_8 = vreg_2;
local_9 = 0;
vreg_108 = vreg_2->Length;
vreg_109 = (int)vreg_108;
goto label_269;
label_18:
vreg_6 = ((*local_8)[local_9]).get();
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
vreg_40 = System_Math_Sqrt(local_6);
vreg_41 = local_6*vreg_40;
local_7 = dt/vreg_41;
vreg_45 = vreg_8->vx;
vreg_113 = vreg_10->mass;
vreg_49 = local_3*vreg_113;
vreg_51 = vreg_49*local_7;
vreg_52 = vreg_45-vreg_51;
vreg_8->vx = vreg_52;
vreg_55 = vreg_10->vx;
vreg_115 = vreg_8->mass;
vreg_59 = local_3*vreg_115;
vreg_61 = vreg_59*local_7;
vreg_62 = vreg_55+vreg_61;
vreg_10->vx = vreg_62;
vreg_65 = vreg_8->vy;
vreg_69 = local_4*vreg_113;
vreg_71 = vreg_69*local_7;
vreg_72 = vreg_65-vreg_71;
vreg_8->vy = vreg_72;
vreg_75 = vreg_10->vy;
vreg_79 = local_4*vreg_115;
vreg_81 = vreg_79*local_7;
vreg_82 = vreg_75+vreg_81;
vreg_10->vy = vreg_82;
vreg_85 = vreg_8->vz;
vreg_89 = local_5*vreg_113;
vreg_91 = vreg_89*local_7;
vreg_92 = vreg_85-vreg_91;
vreg_8->vz = vreg_92;
vreg_95 = vreg_10->vz;
vreg_99 = local_5*vreg_115;
vreg_101 = vreg_99*local_7;
vreg_102 = vreg_95+vreg_101;
vreg_10->vz = vreg_102;
vreg_105 = local_9+1;
local_9 = vreg_105;
label_269:
vreg_111 = (local_9 < vreg_109)?1:0;
if(vreg_111) goto label_18;
return;
}


System_Void _NBodySystem_AdvanceBodies(_NBodySystem * _this, System_Double  dt)
{
_Body * local_0;
Array < std::shared_ptr<_Body> > * local_1;
System_Int32 local_2;
Array < std::shared_ptr<_Body> > * vreg_2;
_Body * vreg_6;
System_Double vreg_9;
System_Double vreg_12;
System_Double vreg_13;
System_Double vreg_14;
System_Double vreg_17;
System_Double vreg_20;
System_Double vreg_21;
System_Double vreg_22;
System_Double vreg_25;
System_Double vreg_28;
System_Double vreg_29;
System_Double vreg_30;
System_Int32 vreg_33;
System_Int32 vreg_36;
System_Int32 vreg_37;
System_Boolean vreg_39;

vreg_2 = _this->bodies.get();
local_1 = vreg_2;
local_2 = 0;
vreg_36 = vreg_2->Length;
vreg_37 = (int)vreg_36;
goto label_86;
label_13:
vreg_6 = ((*local_1)[local_2]).get();
vreg_9 = vreg_6->x;
vreg_12 = vreg_6->vx;
vreg_13 = dt*vreg_12;
vreg_14 = vreg_9+vreg_13;
vreg_6->x = vreg_14;
vreg_17 = vreg_6->y;
vreg_20 = vreg_6->vy;
vreg_21 = dt*vreg_20;
vreg_22 = vreg_17+vreg_21;
vreg_6->y = vreg_22;
vreg_25 = vreg_6->z;
vreg_28 = vreg_6->vz;
vreg_29 = dt*vreg_28;
vreg_30 = vreg_25+vreg_29;
vreg_6->z = vreg_30;
vreg_33 = local_2+1;
local_2 = vreg_33;
label_86:
vreg_39 = (local_2 < vreg_37)?1:0;
if(vreg_39) goto label_13;
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
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

