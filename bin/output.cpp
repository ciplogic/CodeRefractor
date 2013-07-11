#include "sloth.h"
#include "runtime_base.partcpp"
namespace  {
struct NBody;
}
namespace  {
struct Body;
}
namespace  {
struct NBody {
}; }
void _NBody__Main();
namespace  {
struct Body {
 System::Double x;
 System::Double y;
 System::Double z;
 System::Double vx;
 System::Double vy;
 System::Double vz;
 System::Double mass;
 static System::Int32 bodyCount;
}; }
System::Int32 ::Body::bodyCount = 0;
void _NBody__Main()
{
System::Int32 vreg_1;
System::Int32 vreg_2;

vreg_1 = Body::bodyCount;
vreg_2 = vreg_1+1;
Body::bodyCount = vreg_2;
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

