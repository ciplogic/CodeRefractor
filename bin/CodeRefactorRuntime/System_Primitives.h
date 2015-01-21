#ifndef SystemPrimitives_H
#define SystemPrimitives_H

#include <memory>
#include <cstdlib>

/////////////////////////////////////////////////////////////////////////////
//// Defines for various system types.
/////////////////////////////////////////////////////////////////////////////

/**
 * Define the types from the .NET bytecode as C++
 * types.
 */
typedef bool            System_Boolean;
typedef unsigned char   System_Byte;
typedef short		    System_Int16;
typedef int			    System_Int32;
typedef	unsigned int    System_UInt32;
typedef	long long int   System_Int64;
typedef wchar_t		    System_Char;
typedef float		    System_Single;
typedef double		    System_Double;
typedef void* 		    System_IntPtr;
typedef void		    System_Void;

// define the MODULE_HANDLE as either HMODULE (for Windows) or void* for *nix.
#ifdef WIN32
#define MODULE_HANDLE HMODULE
#else // WIN32
#define MODULE_HANDLE void*
#endif // WIN32

static void* IntPtr_Zero = 0;

System_Boolean System_IntPtr_op_Equality(
	const System_IntPtr&src,
	const System_IntPtr&dest)
{
	return src == dest;
}

/**
 * The array implementation for an Array.
 */
template <class T> struct Array
{
	unsigned int Length;
	T* Items;
	Array(int newSize)
	{
		AllocateLength(newSize);
	}
	Array(int newSize,const T* data)
	{
		AllocateLength(newSize);
		for(auto i =0;i<newSize;i++)
			Items[i] = data[i];
	}
	void AllocateLength(int newSize)
	{
		auto sizeT = sizeof(T);
		Items = (T*)calloc(newSize, sizeT);
		Length = newSize;
	}
	T& operator[](const int idx)
	{
		return Items[idx];
	}

	~Array()
	{
		free(Items);
		Items = 0;
	}
};

template <class T> void System_Runtime_CompilerServices_RuntimeHelpers__InitializeArray(
		const std::shared_ptr< Array <T> > & array,
		const std::shared_ptr< Array < System_Byte > > & data
){
	auto arraySrcRef = data.get();
	auto arrayDestRef = array.get();
	auto bytesCount = arrayDestRef->Length * sizeof(T);
	memcpy(arrayDestRef->Items, arraySrcRef->Items, bytesCount);
}

System_Byte* System_Runtime_CompilerServices_RuntimeHelpers__InitializeArray(int id);
void AddConstantByteArray(System_Byte* data);

/////////////////////////////////////////////////////////////////////////////
//// Dynamic linking methods.
/////////////////////////////////////////////////////////////////////////////

#ifndef CODEREFRACTOR_BARE_METAL

// TODO: right now the methods are implemented in runtime_base.hpp, they should be moved out
//MODULE_HANDLE LoadNativeLibrary(const System_Char* dllFileName);
//void* LoadNativeMethod(MODULE_HANDLE module, const char* methodName);

#endif // CODEREFRACTOR_BARE_METAL


#endif // SystemPrimitives_H
