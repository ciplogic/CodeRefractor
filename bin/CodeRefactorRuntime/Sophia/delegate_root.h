
class _never_exist_class_;

// <http://www.boost.org/doc/html/boost/bad_function_call.html>
struct bad_function_call : public std::runtime_error
{
	bad_function_call() : std::runtime_error("call to empty delegate") {}
};

// <http://www.boost.org/libs/ptr_container/doc/reference.html#class-heap-clone-allocator>
struct heap_clone_allocator
{
	template<typename object_type>
		static object_type* allocate_clone(const object_type& r) throw()
	{
		return new object_type(r);
	}

	template<typename object_type>
		static void deallocate_clone(object_type* p) throw()
	{
		// intentionally complex
		// refer to <http://www.boost.org/libs/utility/checked_delete.html>
		typedef char type_must_be_complete[ sizeof(object_type)? 1: -1 ];
		(void) sizeof(type_must_be_complete);

		delete p;
	}
};

// <http://www.boost.org/libs/ptr_container/doc/reference.html#class-view-clone-allocator>
struct view_clone_allocator
{
	template<typename object_type>
		static object_type* allocate_clone(const object_type& r) throw()
	{	return const_cast<object_type*>(&r); }

	static void deallocate_clone(void*)  throw() {}
};

struct com_autoref_clone_allocator
{
	template<typename object_type>
		static object_type* allocate_clone(const object_type& r) throw()
	{
		r.AddRef();
		return const_cast<object_type*>(&r);
	}

	template<typename object_type>
		static void deallocate_clone(object_type* p) throw()
	{
		p->Release();
	}
};

struct delegate_root
{
public:

	// is null?
	bool empty() const throw()
	{
		return strategy().is_empty();
	}

	~delegate_root() throw()
	{
		strategy().object_free();
	}

	////////////////////////////////////////////////////////////////////////
	// comparison
	////////////////////////////////////////////////////////////////////////

	int compare(const delegate_root& _other) const throw()
	{
		return strategy_root::compare(this->strategy(), _other.strategy());
	}

	bool operator==(const delegate_root& _other) const throw()
	{
		return strategy_root::compare(this->strategy(), _other.strategy()) == 0;
	}

	bool operator!=(const delegate_root& _other) const throw()
	{
		return strategy_root::compare(this->strategy(), _other.strategy()) != 0;
	}

	bool operator>(const delegate_root& _other) const throw()
	{
		return strategy_root::compare(this->strategy(), _other.strategy()) > 0;
	}

	bool operator>=(const delegate_root& _other) const throw()
	{
		return strategy_root::compare(this->strategy(), _other.strategy()) >= 0;
	}

	bool operator<(const delegate_root& _other) const throw()
	{
		return strategy_root::compare(this->strategy(), _other.strategy()) < 0;
	}

	bool operator<=(const delegate_root& _other) const throw()
	{
		return strategy_root::compare(this->strategy(), _other.strategy()) <= 0;
	}

	struct strategy_root
	{
		// is null?
		virtual bool is_empty() const throw() { return false; }
		// clone object
		virtual void on_clone_object(void**) const throw() = 0;
		// free object
		virtual void on_free_object(void*) const throw() = 0;

		typedef void (*pointer_to_function)(_never_exist_class_*);

		_never_exist_class_* m_object_ptr;
		union greatest_pointer_type
		{
			typedef void (_never_exist_class_::*pointer_to_method)();
			char unused1[sizeof(pointer_to_function)];
			char unused2[sizeof(pointer_to_method)];
		}	m_fn;

		void init_null() throw()
		{
			COMPILE_TIME_ASSERT(OFFSET_OF(strategy_root,m_object_ptr) < OFFSET_OF(strategy_root,m_fn));
			memset(&m_object_ptr, 0,
				OFFSET_OF(strategy_root,m_fn) + sizeof(m_fn) - OFFSET_OF(strategy_root,m_object_ptr));
		}

		void init_function(pointer_to_function fn) throw()
		{
			m_object_ptr = 0;
			reinterpret_cast<pointer_to_function&>(m_fn) = fn;
			// fill redundant bytes with ZERO
			memset((char*)&m_fn + sizeof(fn), 0, sizeof(m_fn) - sizeof(fn));
		}

		static int compare(const strategy_root& _left,
			const strategy_root& _right) throw()
		{
			COMPILE_TIME_ASSERT(OFFSET_OF(strategy_root,m_fn)
				==(OFFSET_OF(strategy_root,m_object_ptr) + sizeof(_left.m_object_ptr)));

			return memcmp(&(_left.m_object_ptr), &(_right.m_object_ptr),
				sizeof(_left.m_object_ptr) + sizeof(_left.m_fn));
		}

		void object_clone() throw()
		{
			if(0 != this->m_object_ptr)
			{
				this->on_clone_object(&reinterpret_cast<void*&>(m_object_ptr));
			}
		}
		void object_free() throw()
		{
			if(0 != this->m_object_ptr)
			{
				this->on_free_object(m_object_ptr);
			}
		}
	};

protected:

	typedef _never_exist_class_ clear_type;

	strategy_root const& strategy() const throw()
	{
		return *reinterpret_cast<strategy_root const*>(&m_strategy);
	}

	strategy_root& strategy() throw()
	{
		return *reinterpret_cast<strategy_root*>(&m_strategy);
	}

	struct strategy_place_holder
	{
		char unused[sizeof(strategy_root)]; 
	}	m_strategy;
};

template<class clone_allocator_type>
struct clone_option
{
private:
	bool clone_for_first_time;

public:
	clone_option(bool clone_for_first_time = true) throw()
		: clone_for_first_time(clone_for_first_time)
	{
	}

	operator bool() const throw()
	{
		return this->clone_for_first_time;
	}

	template<typename object_type, typename base_type>
	struct strategy : public delegate_root::strategy_root
	{
		// clone object
		virtual void on_clone_object(void** pp) const throw()
		{
			base_type* cloned_ptr = clone_allocator_type::allocate_clone(
				static_cast<object_type&>(*static_cast<base_type*>(*pp)));
			*pp = cloned_ptr;
		}
		// free object
		virtual void on_free_object(void* p) const throw()
		{
			clone_allocator_type::deallocate_clone(
				static_cast<object_type*>(static_cast<base_type*>(p)));
		}
	};
};

template<>
struct clone_option<view_clone_allocator>
{
	operator bool() const throw()
	{
		return false;
	}

	struct no_clone_strategy : public delegate_root::strategy_root
	{
		virtual void on_clone_object(void** pp) const throw()
		{
			// do nothing
		}
		virtual void on_free_object(void* p) const throw()
		{
			// do nothing
		}
	};

	template<typename object_type, typename base_type>
	struct strategy : public no_clone_strategy
	{
	};
};
