#include "stdafx.h"
#include "Sophia/delegate.h"

namespace explanatory_1st_version
{
	class delegate2 // <void (int, char*)>
	{
	protected:
		class _never_exist_class;

		typedef void (_never_exist_class::*thiscall_method)(int, char*);
		typedef void (__cdecl _never_exist_class::*cdecl_method)(int, char*);
		typedef void (__stdcall _never_exist_class::*stdcall_method)(int, char*);
		typedef void (__fastcall _never_exist_class::*fastcall_method)(int, char*);
		typedef void (__cdecl *cdecl_function)(int, char*);
		typedef void (__stdcall *stdcall_function)(int, char*);
		typedef void (__fastcall *fastcall_function)(int, char*);

		enum delegate_type
		{
			thiscall_method_type,
			cdecl_method_type,
			stdcall_method_type,
			fastcall_method_type,
			cdecl_function_type,
			stdcall_function_type,
			fastcall_function_type,
		};

		class greatest_pointer_type
		{
			char never_use[sizeof(thiscall_method)];
		};

		delegate_type m_type;
		_never_exist_class* m_p;
		greatest_pointer_type m_fn;

	public:
		void operator()(int i, char* s)
		{
			switch(m_type)
			{
			case thiscall_method_type:
				return (m_p->*(*(thiscall_method*)(&m_fn)))(i, s);
			case cdecl_function_type:
				return (*(*(cdecl_function*)(&m_fn)))(i, s);
			default:
				// This is just a demo, don't implement for all cases
				throw;
			}
		}

		static int compare(const delegate2& _left, const delegate2& _right)
		{
			// first, compare pointer
			int result = memcmp(&_left.m_fn, &_right.m_fn, sizeof(_left.m_fn));
			if(0 == result)
			{
				// second, compare object
				result = (int) (((char*)_left.m_p) - ((char*)_right.m_p));
			}
			return result;
		}

		// constructor from __cdecl function
		delegate2(void (__cdecl *fn)(int, char*))
		{
			m_type = cdecl_function_type;
			m_p = 0;
			*reinterpret_cast<cdecl_function*>(&m_fn) = fn;
			// fill redundant bytes by ZERO for later comparison
			memset((char*)(&m_fn) + sizeof(fn), 0, sizeof(m_fn) - sizeof(fn));
		}

		// constructor from __thiscall method
		template<typename object_type>
			delegate2(object_type* p, void (object_type::*fn)(int, char*))
		{
			m_type = thiscall_method_type;
			m_p = reinterpret_cast<_never_exist_class*>(p);

			///////////////////////////////////////////////////////////
			// WE WANT DOING THE FOLLWOING ASSIGNMENT
			// m_fn = fn
			// BUT HOW TO DO IN A STANDARD COMPLIANT AND PORTABLE WAY?
			// FOLLOW IS THE ANSWER
			///////////////////////////////////////////////////////////

			// forward reference
			class _another_never_exist_class_;
			typedef void (_another_never_exist_class_::*large_pointer_to_method)(int, char*);
            
			COMPILE_TIME_ASSERT(sizeof(large_pointer_to_method)==sizeof(greatest_pointer_type ));

			// Now tell compiler that '_another_never_exist_class_' is just a 'T' class
			class _another_never_exist_class_ : public object_type {};
        
			reinterpret_cast<large_pointer_to_method&>(m_fn) = fn;

			// Double checking to make sure the compiler doesn't change its mind :-)
			COMPILE_TIME_ASSERT(sizeof(large_pointer_to_method)==sizeof(greatest_pointer_type ));
		}
	};
}

namespace explanatory_2nd_version
{
	// kind_of_class.cpp
	// This file is to demo about different kinds of pointer to members

	class dummy_base1 { };
	class dummy_base2 { };

	class dummy_s : dummy_base1 { };
	// Reach to here, the compiler will recognize dummy_s is a kind of “single inheritance”.
	typedef void (dummy_s::*pointer_to_dummy_s)(void);
	size_t size_of_single = sizeof(pointer_to_dummy_s);

	class dummy_m : dummy_base1, dummy_base2 { };
	// Reach to here, the compiler will recognize dummy_m is a kind of “multiple inheritance”.
	typedef void (dummy_m::*pointer_to_dummy_m)(void);
	size_t size_of_multi = sizeof(pointer_to_dummy_m);

	class dummy_v : virtual dummy_base1 { };
	// Reach to here, the compiler will recognize dummy_v is a kind of “virtual inheritance”.
	typedef void (dummy_v::*pointer_to_dummy_v)(void);
	size_t size_of_virtual = sizeof(pointer_to_dummy_v);

	class dummy_u;
	// forward reference, unknown at this time
	typedef void (dummy_u::*pointer_to_dummy_u)(void);
	size_t size_of_unknown = sizeof(pointer_to_dummy_u);

	void main()
	{
		printf("%d\n%d\n%d\n%d", size_of_single, size_of_multi, size_of_virtual, size_of_unknown);
	}

	class delegate_strategy // <void (int, char*)>
	{
	protected:
		class _never_exist_class;

		typedef void (_never_exist_class::*thiscall_method)(int, char*);
		typedef void (__cdecl _never_exist_class::*cdecl_method)(int, char*);
		typedef void (__stdcall _never_exist_class::*stdcall_method)(int, char*);
		typedef void (__fastcall _never_exist_class::*fastcall_method)(int, char*);
		typedef void (__cdecl *cdecl_function)(int, char*);
		typedef void (__stdcall *stdcall_function)(int, char*);
		typedef void (__fastcall *fastcall_function)(int, char*);

		class greatest_pointer_type
		{
			char never_use[sizeof(thiscall_method)];
		};

		_never_exist_class* m_p;
		greatest_pointer_type m_fn;

	public:

		// pure virtual function
		virtual void operator()(int, char*) const
		{
			throw std::exception();
		}
	};

	class delegate_cdecl_function_strategy : public delegate_strategy
	{
		// concrete strategy
		virtual void operator()(int i, char* s) const
		{
			return (*(*(cdecl_function*)(&m_fn)))(i, s);
		}

	public:

		// constructor
		delegate_cdecl_function_strategy(void (__cdecl *fn)(int, char*))
		{
			m_p = 0;
			*reinterpret_cast<cdecl_function*>(&m_fn) = fn;
			// fill redundant bytes by ZERO for later comparison
			memset((char*)(&m_fn) + sizeof(fn), 0, sizeof(m_fn) - sizeof(fn));
		}
	};

	class delegate_thiscall_method_strategy : public delegate_strategy
	{
		// concrete strategy
		virtual void operator()(int i, char* s) const
		{
			return (m_p->*(*(thiscall_method*)(&m_fn)))(i, s);
		}

	public:

		// constructor
		template<typename object_type>
			delegate_thiscall_method_strategy(object_type* p, void (object_type::*fn)(int, char*))
		{
			m_p = reinterpret_cast<_never_exist_class*>(p);

			///////////////////////////////////////////////////////////
			// WE WANT DOING THE FOLLWOING ASSIGNMENT
			// m_fn = fn
			// BUT HOW TO DO IN A STANDARD COMPLIANT AND PORTABLE WAY?
			// FOLLOW IS THE ANSWER
			///////////////////////////////////////////////////////////

			// forward reference
			class _another_never_exist_class_;
			typedef void (_another_never_exist_class_::*large_pointer_to_method)(int, char*);
            
			COMPILE_TIME_ASSERT(sizeof(large_pointer_to_method)==sizeof(greatest_pointer_type ));

			// Now tell compiler that '_another_never_exist_class_' is just a 'T' class
			class _another_never_exist_class_ : public object_type {};
        
			reinterpret_cast<large_pointer_to_method&>(m_fn) = fn;

			// Double checking to make sure the compiler doesn't change its mind :-)
			COMPILE_TIME_ASSERT(sizeof(large_pointer_to_method)==sizeof(greatest_pointer_type ));
		}
	};

	class delegate2 // <void (int, char*)>
	{
	protected:
		char m_strategy[sizeof(delegate_strategy)];

		const delegate_strategy& strategy() const
		{
			return *reinterpret_cast<delegate_strategy const*>(&m_strategy);
		}

	public:
		// constructor for __cdecl function
		delegate2(void (__cdecl *fn)(int, char*))
		{
			new (&m_strategy) delegate_cdecl_function_strategy(fn);
		}

		// constructor
		template<typename object_type>
			delegate2(object_type* p, void (object_type::*fn)(int, char*))
		{
			new (&m_strategy) delegate_thiscall_method_strategy(p, fn);
		}

		// Syntax 01: (*delegate)(param...)
		delegate_strategy const& operator*() const throw()
		{
			return strategy();
		}

		// Syntax 02: delegate(param...)
		// Note: syntax 02 might be slower than syntax 01 in some cases
		void operator()(int i, char* s) const
		{
			return strategy()(i, s);
		}
	};

	struct CKnownClass
	{
		static void static_method(int i, char* s)
		{
			printf("static_method is called: (param1 = %d) (param2 = %s)\n", i, s);
		}

		void method(int i, char* s)
		{
			printf("method is called: (param1 = %d) (param2 = %s)\n", i, s);
		}
	};

	inline void Test()
	{
		// bind to static method
		delegate2 d1(&CKnownClass::static_method);

		// bind to method
		CKnownClass o;
		delegate2 d2(&o, &CKnownClass::method);

		// Syntax 1
		d1(10, "First call");
		d2(5, "Second call");

		// Syntax 2
		(*d1)(10, "First call");
		(*d2)(5, "Second call");
	}
}

namespace final_version
{
	using namespace sophia;

	// Base class with a virtual method
	struct BaseClass
	{
		virtual int virtual_method(int param) const
		{
			printf("We are in BaseClass: (param = %d)\n", param);
			return param;
		}

		char relaxed_method(long param)
		{
			printf("We are in relaxed_method: (param = %d)\n", param);
			return 0;
		}
	};

	// A virtual-inheritance class
	struct DerivedClass : public virtual BaseClass
	{
		virtual int virtual_method(int param) const
		{
			printf("We are in DerivedClass: (param = %d)\n", param);
			return param;
		}
	};

	void Test()
	{
		// Assuming we have some objects
		DerivedClass object;

		// Delegate declaration
		typedef sophia::delegate0<DWORD> MyDelegate0;
		typedef sophia::delegate1<int, int> MyDelegate1;
		typedef sophia::delegate4<void, int, int, long, char> AnotherDelegateType;

		// Determine size of a delegate instance
		printf("sizeof(delegate) = %d\n", sizeof(AnotherDelegateType));

		// Constructor
		MyDelegate0 d0(&GetCurrentThreadId);
		MyDelegate1 d1(&object, &DerivedClass::virtual_method);
		MyDelegate1 d2; // null delegate
		AnotherDelegateType dNull;

		// Compare between delegates even if they are different types
		assert(d2 == dNull);

		// Bind to a free function or a method
		d0.bind(&GetCurrentThreadId);
		d0 = &GetCurrentThreadId;
		d2.bind(&object, &DerivedClass::virtual_method);

		// Compare again after binding
		assert(d2 == d1);

		// Clear a delegate
		d2 = NULL; // or
		d2.clear();

		// Invoke with syntax 01
		d1(1000);

		// Invoke with syntax 02
		// This syntax is faster than syntax 01
		(*d1)(10000);

		// RELAXED delegate
		d1.bind(&object, &DerivedClass::relaxed_method);
		(*d1)(10000);

		// Swap between two delegates
		d2.swap(d1); // now d1 == NULL

		// Execute a null/empty delegate
		assert(d1.empty());
		try
		{
			d1(100);
		}
		catch(sophia::bad_function_call& e)
		{
			printf("Exception: %s\n    Try again: ", e.what());
			d2(0);
		}

		// Object life-time management
		// Case 1: we want the object is cloned
		d1.bind(&object, &DerivedClass::virtual_method, 
			clone_option<heap_clone_allocator>(true));

		// Object life-time management
		// Case 2: we DO NOT want the object is cloned when binding
		for(int i=0; i<100; ++i)
		{
			DerivedClass* pObject = new DerivedClass();
			d1.bind(pObject, &DerivedClass::virtual_method, 
				clone_option<heap_clone_allocator>(false));
			d1(100);
		}
	}
}

int main(int argc, char* args[])
{
	printf("\n--------------------Explanatory version--------------\n\n");
	explanatory_2nd_version::Test();
	printf("\n--------------------Final version--------------------\n\n");
	final_version::Test();

	return 0;
}