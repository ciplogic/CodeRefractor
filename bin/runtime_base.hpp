#include <vector>
#include <map>
#include <string>
#include <memory>

#ifdef CODEREFRACTOR_BARE_METAL
#define CODEREFRACTOR_NO_DYNAMIC_LINKING
#endif // CODEREFRACTOR_BARE_METAL

static std::vector<std::shared_ptr<System_String> > _stringJumps;

//void buildTypesTable();
void buildStringTable();
void mapLibs();
void RuntimeHelpersBuildConstantTable();
bool IsInstanceOf(int typeSource, int typeImplementation);


void initializeRuntime()
{
	//buildTypesTable();
	buildStringTable();
	mapLibs();
	RuntimeHelpersBuildConstantTable();
}

/**
 * Resize the current array.
 */
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


template<class T>
struct BoxedT : public System_Object
{
	T Data;
};

template<class T>
std::shared_ptr<System_Object> box_value(T value, int typeId){
	auto result = std::make_shared<BoxedT< T > >();
	result->_typeId = typeId;
	result->Data = value;
	return result;
}

template<class T>
T unbox_value(std::shared_ptr<System_Object> value){
	auto resultObject = value.get();
	auto castedUnboxing = (BoxedT<T>*)resultObject;
	return castedUnboxing->Data;
}


extern const wchar_t _stringTable[];
std::shared_ptr<System_String> _str(int index)
{
	return _stringJumps[index];
}

void _AddJumpAndLength(int jump, int length)
{
	auto resultData = &(_stringTable[jump]);

	auto result = std::make_shared<System_String>();
	auto value = std::make_shared<Array < System_Char >>(length+1, resultData);
	result->Text =  value;
	_stringJumps.push_back(result);
}

static std::vector<System_Byte*> _constTables;
System_Byte* RuntimeHelpers_GetBytes(int id)
{
	return _constTables[id];
}

void AddConstantByteArray(System_Byte* data)
{
	_constTables.push_back(data);
}

/////////////////////////////////////////////////////////////////////////////
//// Dynamic Loading.
/////////////////////////////////////////////////////////////////////////////

#ifndef CODEREFRACTOR_BARE_METAL
    #ifdef CODEREFRACTOR_NO_DYNAMIC_LINKING
        void fail_application_spectaculously();

        void* LoadNativeLibrary(const char* dllFileName)
        {
            fail_application_spectaculously();
        }

        void* LoadNativeMethod(void* module, const char* methodName)
        {
            fail_application_spectaculously();
        }

        /**
         * Just sigsegvs on purpose, since we want to stop the application but we
         * can't depend on anything external, and no dynamic linking is available.
         */
        void fail_application_spectaculously()
        {
            char* null_reference = NULL;
            null_reference[0] = 0; // fail on purpose.
        }
    #else // not CODEREFRACTOR_NO_DYNAMIC_LINKING
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
        #else // not _WIN32
            #include <dlfcn.h>

            void* LoadNativeLibrary(const char* dllFileName)
            {
                return dlopen(dllFileName, RTLD_LAZY);
            }

            void* LoadNativeMethod(void* module, const char* methodName)
            {
                return (void*)dlsym(module, methodName);
            }
        #endif // not _WIN32
    #endif // not CODEREFRACTOR_NO_DYNAMIC_LINKING
#endif // not CODEREFRACTOR_BARE_METAL

#include <stdio.h>
#include <math.h>

std::shared_ptr<Array < std::shared_ptr<System_String> > > System_getArgumentsAsList(int argc, char**argv)
{
	auto result = new Array <std::shared_ptr<System_String> >(argc);
	for(auto i=0;i<argc;i++){
		//std::shared_ptr<System::String> newString (new System::String(argv[i]));
		//(*result)[i] = newString;
	}
	return std::shared_ptr< Array <std::shared_ptr<System_String> > >(result);
}


void System_Console__WriteLine(const wchar_t* value)
{
	wprintf(L"%ls\n", value);
}
