#ifndef SystemConsole_H
#define SystemConsole_H

#include "../System_Primitives.h"
#include <math.h>
#include <string.h>

#include <memory>

namespace System
{
	struct String;
	std::shared_ptr< Array < System::String> > getArgumentsAsList(int argc, char**argv);

}


#endif
