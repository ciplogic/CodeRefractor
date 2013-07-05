#ifndef SystemConsole_H
#define SystemConsole_H

#include "System_Primitives.h"
#include <math.h>

#include <memory>

namespace System
{
	struct String {
		std::shared_ptr< Array <System::Char > > _data;
		String() {}
		
		String(char* source){}
	};
/*	class Console
	{
	public:
		static void WriteLine(System::Int32 value);
		static void Write(System::Int32 value);
		static void WriteLine(System::Double value);
		static void Write(System::Double value);
	};
*/	
	std::shared_ptr< Array < System::String> > getArgumentsAsList (int argc, char**argv);
	/*
	struct Math{
		inline static double Sqrt(double value) {
			return sqrt(value);
		}

	};
*/
}
#endif
