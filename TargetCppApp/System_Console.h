#pragma once

#include "System_Primitives.h"

#include <memory>

namespace System
{
	struct String;
	std::shared_ptr< Array < System::String> > getArgumentsAsList(int argc, char**argv);

}
