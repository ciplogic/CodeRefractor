#include "System_Console.h"

#include <stdio.h>
#include <math.h>

std::shared_ptr< Array < System::String> > System::getArgumentsAsList (int argc, char**argv)
{
	auto result = std::make_shared<Array < System::String> >(argc);
	for(auto i=0;i<argc;i++){
		(*result)[i] = System::String(argv[i]);
	}
	return result;
}
