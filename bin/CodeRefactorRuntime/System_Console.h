#ifndef SystemConsole_H
#define SystemConsole_H

#include "System_Primitives.h"
#include <math.h>

#include <memory>

namespace System
{
	struct String {
		System::Char* _data;
		String() {
			_data = 0;
		}
		~String() {
			delete []_data;
		}
		String(char* source){
			int len = strlen(source);
			System::Char* data = new System::Char[len+1];
			for(auto i=0; i<=len; i++)
			{
				data[i] = source[i];
			}
			Initialize(len, data);
			delete []data;
		}
		String(int len, const System::Char* data){
			Initialize(len, data);			
		}

		void Initialize(int len, const System::Char* data)
		{
			_data = new System::Char [len+3];
			
			*((int*)_data)=len;
			memcpy(&_data[2], data, len*2);
			_data[len+2]=0;
		}
		
		System::Char* get(){
			return & _data[2];
		}
	};
	std::shared_ptr<Array < std::shared_ptr<System::String> > > getArgumentsAsList (int argc, char**argv);

}
#endif
