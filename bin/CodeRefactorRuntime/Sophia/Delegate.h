#pragma once

#include <memory.h>
#include <stdexcept>
#include "compile_time_util.h"

#pragma push_macro("new")
#undef new

#if defined(_MSC_VER)
#define SOPHIA_SUPPORT_CALLING_CONVENTION_FOR_METHOD
#endif

#define SOPHIA_DELEGATE_CLASSNAME   COMPILE_TIME_JOIN(delegate,SOPHIA_PARAM_COUNT)
#define SOPHIA_DETAIL_NAMESPACE		COMPILE_TIME_JOIN(detail,SOPHIA_PARAM_COUNT)

#define SOPHIA_MAKE_PARAMS1_0(t)	
#define SOPHIA_MAKE_PARAMS1_1(t)    t##1
#define SOPHIA_MAKE_PARAMS1_2(t)    t##1, ##t##2
#define SOPHIA_MAKE_PARAMS1_3(t)    t##1, ##t##2, ##t##3
#define SOPHIA_MAKE_PARAMS1_4(t)    t##1, ##t##2, ##t##3, ##t##4
#define SOPHIA_MAKE_PARAMS1_5(t)    t##1, ##t##2, ##t##3, ##t##4, ##t##5
#define SOPHIA_MAKE_PARAMS1_6(t)    t##1, ##t##2, ##t##3, ##t##4, ##t##5, ##t##6
#define SOPHIA_MAKE_PARAMS1_7(t)    t##1, ##t##2, ##t##3, ##t##4, ##t##5, ##t##6, ##t##7
#define SOPHIA_MAKE_PARAMS1_8(t)    t##1, ##t##2, ##t##3, ##t##4, ##t##5, ##t##6, ##t##7, ##t##8
#define SOPHIA_MAKE_PARAMS1_9(t)    t##1, ##t##2, ##t##3, ##t##4, ##t##5, ##t##6, ##t##7, ##t##8, ##t##9
#define SOPHIA_MAKE_PARAMS1_10(t)   t##1, ##t##2, ##t##3, ##t##4, ##t##5, ##t##6, ##t##7, ##t##8, ##t##9, ##t##10

#define SOPHIA_MAKE_PARAMS2_0(t1, t2)	
#define SOPHIA_MAKE_PARAMS2_1(t1, t2)   t1##1 t2##1
#define SOPHIA_MAKE_PARAMS2_2(t1, t2)   t1##1 t2##1, t1##2 t2##2
#define SOPHIA_MAKE_PARAMS2_3(t1, t2)   t1##1 t2##1, t1##2 t2##2, t1##3 t2##3
#define SOPHIA_MAKE_PARAMS2_4(t1, t2)   t1##1 t2##1, t1##2 t2##2, t1##3 t2##3, t1##4 t2##4
#define SOPHIA_MAKE_PARAMS2_5(t1, t2)   t1##1 t2##1, t1##2 t2##2, t1##3 t2##3, t1##4 t2##4, t1##5 t2##5
#define SOPHIA_MAKE_PARAMS2_6(t1, t2)   t1##1 t2##1, t1##2 t2##2, t1##3 t2##3, t1##4 t2##4, t1##5 t2##5, t1##6 t2##6
#define SOPHIA_MAKE_PARAMS2_7(t1, t2)   t1##1 t2##1, t1##2 t2##2, t1##3 t2##3, t1##4 t2##4, t1##5 t2##5, t1##6 t2##6, t1##7 t2##7
#define SOPHIA_MAKE_PARAMS2_8(t1, t2)   t1##1 t2##1, t1##2 t2##2, t1##3 t2##3, t1##4 t2##4, t1##5 t2##5, t1##6 t2##6, t1##7 t2##7, t1##8 t2##8
#define SOPHIA_MAKE_PARAMS2_9(t1, t2)   t1##1 t2##1, t1##2 t2##2, t1##3 t2##3, t1##4 t2##4, t1##5 t2##5, t1##6 t2##6, t1##7 t2##7, t1##8 t2##8, t1##9 t2##9
#define SOPHIA_MAKE_PARAMS2_10(t1, t2)  t1##1 t2##1, t1##2 t2##2, t1##3 t2##3, t1##4 t2##4, t1##5 t2##5, t1##6 t2##6, t1##7 t2##7, t1##8 t2##8, t1##9 t2##9, t1##10 t2##10

#define SOPHIA_TEMPLATE_PARAMS    SOPHIA_MAKE_PARAMS1(SOPHIA_PARAM_COUNT,typename A)
// typename A0, class A1, class A2, ...
#define SOPHIA_TEMPLATE_ARGS      SOPHIA_MAKE_PARAMS1(SOPHIA_PARAM_COUNT,A)
// A0, A1, A2, ...
#define SOPHIA_PARAMS             SOPHIA_MAKE_PARAMS2(SOPHIA_PARAM_COUNT,A,a)
// A0 a0, A1 a1, A2 a2, ...
#define SOPHIA_ARGS               SOPHIA_MAKE_PARAMS1(SOPHIA_PARAM_COUNT,a)
// a0, a1, a2, ...

#define SOPHIA_TEMPLATE_PARAMS_RELAX    SOPHIA_MAKE_PARAMS1(SOPHIA_PARAM_COUNT,typename R_A)
#define SOPHIA_TEMPLATE_ARGS_RELAX      SOPHIA_MAKE_PARAMS1(SOPHIA_PARAM_COUNT,R_A)

#define SOPHIA_MAKE_PARAMS1(n, t)         COMPILE_TIME_JOIN(SOPHIA_MAKE_PARAMS1_, n) (t)
#define SOPHIA_MAKE_PARAMS2(n, t1, t2)    COMPILE_TIME_JOIN(SOPHIA_MAKE_PARAMS2_, n) (t1, t2)

namespace sophia
{

#include "delegate_root.h"

// 0 params
#define SOPHIA_PARAM_COUNT 0
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 1 params
#define SOPHIA_PARAM_COUNT 1
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 2 params
#define SOPHIA_PARAM_COUNT 2
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 3 params
#define SOPHIA_PARAM_COUNT 3
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 4 params
#define SOPHIA_PARAM_COUNT 4
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 5 params
#define SOPHIA_PARAM_COUNT 5
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 6 params
#define SOPHIA_PARAM_COUNT 6
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 7 params
#define SOPHIA_PARAM_COUNT 7
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 8 params
#define SOPHIA_PARAM_COUNT 8
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 9 params
#define SOPHIA_PARAM_COUNT 9
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

// 10 params
#define SOPHIA_PARAM_COUNT 10
#include "delegate_template.h"
#undef SOPHIA_PARAM_COUNT

} // end namespace

#undef SOPHIA_DELEGATE_CLASSNAME
#undef SOPHIA_DETAIL_NAMESPACE

#undef SOPHIA_TEMPLATE_PARAMS
#undef SOPHIA_TEMPLATE_ARGS
#undef SOPHIA_PARAMS
#undef SOPHIA_ARGS

#undef SOPHIA_MAKE_PARAMS1
#undef SOPHIA_MAKE_PARAMS2

#undef SOPHIA_MAKE_PARAMS1_0
#undef SOPHIA_MAKE_PARAMS1_1
#undef SOPHIA_MAKE_PARAMS1_2
#undef SOPHIA_MAKE_PARAMS1_3
#undef SOPHIA_MAKE_PARAMS1_4
#undef SOPHIA_MAKE_PARAMS1_5
#undef SOPHIA_MAKE_PARAMS1_6
#undef SOPHIA_MAKE_PARAMS1_7
#undef SOPHIA_MAKE_PARAMS1_8
#undef SOPHIA_MAKE_PARAMS1_9
#undef SOPHIA_MAKE_PARAMS1_10

#undef SOPHIA_MAKE_PARAMS2_0
#undef SOPHIA_MAKE_PARAMS2_1
#undef SOPHIA_MAKE_PARAMS2_2
#undef SOPHIA_MAKE_PARAMS2_3
#undef SOPHIA_MAKE_PARAMS2_4
#undef SOPHIA_MAKE_PARAMS2_5
#undef SOPHIA_MAKE_PARAMS2_6
#undef SOPHIA_MAKE_PARAMS2_7
#undef SOPHIA_MAKE_PARAMS2_8
#undef SOPHIA_MAKE_PARAMS2_9
#undef SOPHIA_MAKE_PARAMS2_10
#undef SOPHIA_SUPPORT_CALLING_CONVENTION_FOR_METHOD

#pragma pop_macro("new")
