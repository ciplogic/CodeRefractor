#if SOPHIA_PARAM_COUNT > 0
#define SOPHIA_COMMA ,
#else
#define SOPHIA_COMMA 
#endif // SOPHIA_PARAM_COUNT

namespace SOPHIA_DETAIL_NAMESPACE
{
	template<typename R>
	struct relax_if_void // R != void
	{
		template<typename base_class, typename TFunction SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
		struct function_strategy : public base_class
		{
			virtual R operator() (SOPHIA_PARAMS) const
			{
				return (*(*(TFunction*)(&this->m_fn)))(SOPHIA_ARGS);
			}
		};

		template<typename base_class, typename calling_convention SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
		struct method_strategy : public base_class
		{
			virtual R operator() (SOPHIA_PARAMS) const
			{
				return (this->m_object_ptr->*(*(typename calling_convention::rebind::method*)(&this->m_fn)))(SOPHIA_ARGS);
			}
		};
	};

	template<>
	struct relax_if_void<void> // R == void
	{
		template<typename base_class, typename TFunction SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
		struct function_strategy : public base_class
		{
			virtual void operator() (SOPHIA_PARAMS) const
			{
				(*(*(TFunction*)(&this->m_fn)))(SOPHIA_ARGS);
			}
		};

		template<typename base_class, typename calling_convention SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
		struct method_strategy : public base_class
		{
			virtual void operator() (SOPHIA_PARAMS) const
			{
				(this->m_object_ptr->*(*(typename calling_convention::rebind::method*)(&this->m_fn)))(SOPHIA_ARGS);
			}
		};
	};

	template<typename _object_type, typename R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
	struct thiscall_convention
	{
		typedef _object_type object_type;
		typedef R (_object_type::*method)(SOPHIA_TEMPLATE_ARGS);
		typedef R (_object_type::*const_method)(SOPHIA_TEMPLATE_ARGS) const;
		typedef thiscall_convention<_never_exist_class_, R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS> rebind;
	};

	template<typename _object_type, typename R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
	struct cdecl_convention
	{
		typedef _object_type object_type;
		typedef R (__cdecl _object_type::*method)(SOPHIA_TEMPLATE_ARGS);
		typedef R (__cdecl _object_type::*const_method)(SOPHIA_TEMPLATE_ARGS) const;
		typedef cdecl_convention<_never_exist_class_, R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS> rebind;
	};

	template<typename _object_type, typename R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
	struct stdcall_convention
	{
		typedef _object_type object_type;
		typedef R (__stdcall _object_type::*method)(SOPHIA_TEMPLATE_ARGS);
		typedef R (__stdcall _object_type::*const_method)(SOPHIA_TEMPLATE_ARGS) const;
		typedef stdcall_convention<_never_exist_class_, R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS> rebind;
	};

	template<typename _object_type, typename R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
	struct fastcall_convention
	{
		typedef _object_type object_type;
		typedef R (__fastcall _object_type::*method)(SOPHIA_TEMPLATE_ARGS);
		typedef R (__fastcall _object_type::*const_method)(SOPHIA_TEMPLATE_ARGS) const;
		typedef fastcall_convention<_never_exist_class_, R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS> rebind;
	};
}

template<typename R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS>
class SOPHIA_DELEGATE_CLASSNAME : public delegate_root
{
protected:

	typedef clone_option<view_clone_allocator>::no_clone_strategy no_clone_strategy;

	struct invokable_strategy : public no_clone_strategy
	{
		virtual R operator() (SOPHIA_PARAMS) const = 0;
	};

	struct null_strategy : public no_clone_strategy
	{
		null_strategy()
		{
			this->init_null();
		}
		virtual R operator() (SOPHIA_PARAMS) const
		{
			throw bad_function_call();
		}
		virtual bool is_empty() const throw()
		{
			return true;
		}
	};

	template<typename TFunction>
	struct function_strategy : public SOPHIA_DETAIL_NAMESPACE::relax_if_void<R>::template
		function_strategy<no_clone_strategy, TFunction SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS>
	{
		function_strategy(TFunction fn)
		{
			COMPILE_TIME_ASSERT(sizeof(strategy_root::pointer_to_function)==sizeof(TFunction));
			this->init_function(reinterpret_cast<strategy_root::pointer_to_function&>(fn));
		}
	};

	template<typename clone_strategy_type, typename calling_convention>
	struct method_strategy : public SOPHIA_DETAIL_NAMESPACE::relax_if_void<R>::template
		method_strategy<clone_strategy_type, calling_convention SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS>
	{
		method_strategy(typename calling_convention::method fn,
			typename calling_convention::object_type & r)
		{
			this->m_object_ptr = reinterpret_cast<_never_exist_class_*>(&r);

			// translate pointer-to-member to its large form
			class _another_never_exist_class_;
			typedef typename calling_convention::object_type object_type;
			typedef	void (object_type::*small_pointer_to_method)();
			typedef void (_another_never_exist_class_::*large_pointer_to_method)();

			COMPILE_TIME_ASSERT(
				(sizeof(small_pointer_to_method)==sizeof(typename calling_convention::method)) &&
				(sizeof(large_pointer_to_method)==sizeof(strategy_root::greatest_pointer_type)));

			// Now tell the compiler about '_another_never_exist_class_"
			class _another_never_exist_class_ : public object_type {};

			// Double checking to make sure the compiler doesn't change its mind :-)
			COMPILE_TIME_ASSERT(
				(sizeof(small_pointer_to_method)==sizeof(typename calling_convention::method)) &&
				(sizeof(large_pointer_to_method)==sizeof(strategy_root::greatest_pointer_type)));

			reinterpret_cast<large_pointer_to_method&>(this->m_fn) = reinterpret_cast<const small_pointer_to_method&>(fn);
		}
	};

	template<typename clone_strategy_type, typename calling_convention>
	struct const_method_strategy : public method_strategy<clone_strategy_type, calling_convention>
	{
		// treat const method just as non-const method
		// ==> forward all parameter to its base class after some necessary castings
		const_method_strategy(typename calling_convention::const_method fn,
			typename calling_convention::object_type const& r)
			: method_strategy<clone_strategy_type, calling_convention>(
			reinterpret_cast<typename calling_convention::method&>(fn),
			const_cast<typename calling_convention::object_type&>(r))
		{
		}
	};

	// construct delegate from 'thiscall' method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX), object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::thiscall_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}

	// construct delegate from 'thiscall' const-method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX) const, object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) const_method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::thiscall_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}

#if defined(SOPHIA_SUPPORT_CALLING_CONVENTION_FOR_METHOD)

	// construct delegate from 'cdecl' method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (__cdecl base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX), object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::cdecl_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}

	// construct delegate from 'cdecl' const-method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (__cdecl base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX) const, object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) const_method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::cdecl_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}

	// construct delegate from 'stdcall' method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (__stdcall base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX), object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::stdcall_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}

	// construct delegate from 'stdcall' const-method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (__stdcall base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX) const, object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) const_method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::stdcall_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}

	// construct delegate from 'fastcall' method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (__fastcall base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX), object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::fastcall_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}

	// construct delegate from 'fastcall' const-method
	template<typename base_type, typename R_R SOPHIA_COMMA SOPHIA_TEMPLATE_PARAMS_RELAX, typename object_type, typename clone_info>
		void construct_method(R_R (__fastcall base_type::*fn)(SOPHIA_TEMPLATE_ARGS_RELAX) const, object_type& r, clone_info info) throw()
	{
		new (&this->m_strategy) const_method_strategy<typename clone_info::template strategy<object_type, base_type>,
			SOPHIA_DETAIL_NAMESPACE::fastcall_convention<base_type, R_R SOPHIA_COMMA SOPHIA_TEMPLATE_ARGS_RELAX> >(fn, r);
		if(info) this->strategy().object_clone();
	}
#endif //defined(SOPHIA_SUPPORT_CALLING_CONVENTION_FOR_METHOD)

public:

	// default constructor
	SOPHIA_DELEGATE_CLASSNAME() throw()
	{
		new (&this->m_strategy) null_strategy();
	}

	// copy constructor
	SOPHIA_DELEGATE_CLASSNAME(SOPHIA_DELEGATE_CLASSNAME const & _other) throw()
	{
		this->m_strategy = _other.m_strategy; // do bit-wise copy
		this->strategy().object_clone(); // then clone object
	}

	// operator =
	SOPHIA_DELEGATE_CLASSNAME& operator=(SOPHIA_DELEGATE_CLASSNAME const & _other) throw()
	{
		delegate_root old(*this); // cleaning up automatically on return 
		new (this) SOPHIA_DELEGATE_CLASSNAME(_other);;
		return *this;
	}

	// swap between 2 delegates
	void swap(SOPHIA_DELEGATE_CLASSNAME& _other) throw()
	{
		// all operations are just bit-wise copy
		strategy_place_holder temp(this->m_strategy);
		this->m_strategy = _other.m_strategy;
		_other.m_strategy = temp;
	}

	// reset to null
	void clear() throw()
	{
		this->strategy().object_free();
		new (&this->m_strategy) null_strategy();
	}

	// operator = NULL
	const SOPHIA_DELEGATE_CLASSNAME& operator=(clear_type*)
	{
		this->clear();
		return *this;
	}

	// Syntax 01: (*delegate)(param...)
	invokable_strategy const& operator*() const throw()
	{
		return static_cast<invokable_strategy const&>(this->strategy());
	}

	// Syntax 02: delegate(param...)
	// Note: syntax 02 might be slower than syntax 01 in some cases
	R operator()(SOPHIA_PARAMS) const
	{
		return static_cast<invokable_strategy const&>(this->strategy())(SOPHIA_ARGS);
	}

	// constructor for function
	template<typename TFunction>
		SOPHIA_DELEGATE_CLASSNAME(TFunction fn)
	{
		new (&this->m_strategy) function_strategy<TFunction>(fn);
	}

	// bind to function
	template<typename TFunction>
		void bind(TFunction fn)
	{
		this->strategy().object_free();
		new (&this->m_strategy) function_strategy<TFunction>(fn);
	}

	// constructor for method
	template<typename TMethod, typename object_type>
		SOPHIA_DELEGATE_CLASSNAME(object_type* p, TMethod fn)
	{
		this->construct_method(fn, *p, clone_option<view_clone_allocator>());
	}

	// bind to method
	template<typename TMethod, typename object_type>
		void bind(object_type* p, TMethod fn)
	{
		this->strategy().object_free();
		this->construct_method(fn, *p, clone_option<view_clone_allocator>());
	}

	// constructor for method using clone-allocator
	template<typename TMethod, typename object_type, typename clone_info>
		SOPHIA_DELEGATE_CLASSNAME(object_type* p, TMethod fn, clone_info info)
	{
		this->construct_method(fn, *p, info);
	}

	// bind to method using clone-allocator
	template<typename TMethod, typename object_type, typename clone_info>
		void bind(object_type* p, TMethod fn, clone_info info)
	{
		this->strategy().object_free();
		this->construct_method(fn, *p, info);
	}
};

#undef SOPHIA_COMMA
