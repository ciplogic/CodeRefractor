#pragma once

///////////////////////////////////////////////////////////////////////////////////
// COMPILE_TIME_JOIN macro
// Identical to BOOST_JOIN

#define COMPILE_TIME_JOIN(X, Y)		COMPILE_TIME_JOIN1(X, Y)
#define COMPILE_TIME_JOIN1(X, Y)	COMPILE_TIME_JOIN2(X, Y)
#define COMPILE_TIME_JOIN2(X, Y)	X##Y

///////////////////////////////////////////////////////////////////////////////////
// COMPILE_TIME_ASSERT macro

namespace compile_time_util
{

template<bool = true> class assert_class
{
public:
	class private_class_if_failed{};
};

template<> class assert_class<false>
{
private:
	class private_class_if_failed{};
};

}

#if defined(_MSC_VER)
#define COMPILE_TIME_ASSERT(expr) class COMPILE_TIME_JOIN(COMPILE_TIME_JOIN(noname_struct_,__LINE__), __COUNTER__) : public compile_time_util::assert_class<(bool)(expr)>::private_class_if_failed {}
#else
#define COMPILE_TIME_ASSERT(expr) class COMPILE_TIME_JOIN(noname_struct_,__LINE__) : public compile_time_util::assert_class<(bool)(expr)>::private_class_if_failed {}
#endif


///////////////////////////////////////////////////////////////////////////////////
// OFFSET_OF macro
// A workaround to avoid problem with standard 'offsetof' when compiling with G++

#if defined(__GNUG__)
#define GCC_OFFSETOF_KEYWORD __offsetof__
#else
#define GCC_OFFSETOF_KEYWORD
#endif

#define OFFSET_OF(TYPE, MEMBER)							\
	(GCC_OFFSETOF_KEYWORD (reinterpret_cast <size_t>	\
	(&reinterpret_cast <const volatile char &>			\
	(reinterpret_cast<TYPE *> (1)->MEMBER))) - 1)
