#include <vector>
#include <string>
#include <memory>
#include <map>
#include <cwchar>


void buildStringTable();
void mapLibs();
void RuntimeHelpersBuildConstantTable();


void initializeRuntime()
{
	buildStringTable();
	mapLibs();
	RuntimeHelpersBuildConstantTable();
}

template<class T>
void System_Array_Resize(std::shared_ptr< Array<T> >* arr, int newSize)
{
	auto oldSize = (*arr)->Length;
	(*arr)->Length = newSize;
	for(auto i = newSize; i<oldSize;i++)
	{
		T cleanInstance;
		(*arr)->Items[i] = cleanInstance;
	}
	(*arr)->Items = (T*)realloc((*arr)->Items, sizeof(T)*newSize);
}

#ifdef _WIN32

#include <windows.h>

HMODULE LoadNativeLibrary(const System_Char* dllFileName)
{
	return LoadLibraryW(dllFileName);
}

void* LoadNativeMethod(HMODULE module, const char* methodName)
{
	return (void*)GetProcAddress(module, methodName);
}
#else
#include <dlfcn.h>

void* LoadNativeLibrary(const char* dllFileName)
{
	return dlopen(dllFileName, RTLD_LAZY);
}

void* LoadNativeMethod(void* module, const char* methodName)
{
	return (void*)dlsym(module, methodName);
}
#endif

#include <stdio.h>
#include <math.h>

std::shared_ptr<Array < std::shared_ptr<System::String> > > System_getArgumentsAsList (int argc, char**argv)
{
	auto result = new Array <std::shared_ptr<System::String> > (argc);
	for(auto i=0;i<argc;i++){
		//std::shared_ptr<System::String> newString (new System::String(argv[i])); 
		//(*result)[i] = newString;
	}
	return std::shared_ptr< Array <std::shared_ptr<System::String> > >(result);
}


void System_Console__WriteLine(const wchar_t* value)
{
	wprintf(L"%ls\n", value);	
}

/*
char*  GetCString(std::shared_ptr<System_String> _string)
{
	auto length = _string->Text->Length;
	char* tempStr = (char*)calloc(length, sizeof(wchar_t));
	sprintf(tempStr, length, "%ls", _string->Text->Items);
	return tempStr;
}*/






