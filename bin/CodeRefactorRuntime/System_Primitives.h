#ifndef SystemPrimitives_H
#define SystemPrimitives_H

#include <memory>

namespace System {
typedef bool		Boolean;
typedef unsigned char		Byte;
typedef short		Int16;
typedef int			Int32;
typedef	unsigned int UInt32;
typedef	long long int Int64;
typedef wchar_t		Char;
typedef float		Single;
typedef double		Double;
typedef void*		IntPtr;
typedef void		Void;

static void* IntPtr_Zero = 0;


}

System::Boolean System_IntPtr__op_Equality(
	const System::IntPtr&src, 
	const System::IntPtr&dest)
{
	return src == dest;
}

template <class T> struct Array 
{
	unsigned int Length;
	T* Items;
	Array(int newSize)
	{
		Items = new T[newSize];
		Length = newSize;
	}
	Array(int newSize,const T* data)
	{
		Items = new T[newSize];
		Length = newSize;
		for(auto i =0;i<newSize;i++)
			Items[i] = data[i];
	}
	T& operator[](const int idx)
	{
		return Items[idx];
	}

	~Array()
	{
		delete []Items;
		Items = 0;
	}
};

template <class T> void System_Runtime_CompilerServices_RuntimeHelpers__InitializeArray(
		const std::shared_ptr< Array <T> > & array,
		const std::shared_ptr< Array < System::Byte > > & data
){
	auto arraySrcRef = data.get();
	auto arrayDestRef = array.get();
	auto bytesCount = arrayDestRef->Length * sizeof(T);
	memcpy(arrayDestRef->Items, arraySrcRef->Items, bytesCount);
}

System::Byte* System_Runtime_CompilerServices_RuntimeHelpers__InitializeArray(int id);

void AddConstantByteArray(System::Byte* data);
#ifdef WIN32
#include <Windows.h>

HMODULE LoadNativeLibrary(const System::Char* dllFileName);
void* LoadNativeMethod(HMODULE module, const char* methodName);
#else

#endif

#endif
