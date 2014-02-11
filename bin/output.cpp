#include "sloth.h"
struct System_Array; 
struct System_Object; 
struct Game_App; 
struct System_Math; 
struct System_Console; 
struct System_ValueType; 
struct System_IO_File; 
struct System_String; 
struct System_Array {
};
struct System_Object {
};
struct Game_App {
};
struct System_Math {
};
struct System_Console {
};
struct System_ValueType {
};
struct System_IO_File {
};
struct System_String {
 std::shared_ptr< Array < System_Char > > Text;
};
template <class T1> 
struct Game_MyList {
 std::shared_ptr< Array < T1  > > items;
 System_Int32 AutoNamed_0;
 System_Int32 AutoNamed_1;
};

System_Void Game_App_Main();

template <class T1> 
System_Void Game_MyList_ctor(Game_MyList<T1> * _this);

template <class T1> 
System_Void Game_MyList_Add(const std::shared_ptr<Game_MyList<T1>>& _this, T1  item);

#include "runtime_base.hpp"
///---Begin closure code --- 
System_Void Game_App_Main()
{
std::shared_ptr<Game_MyList <System_Int32> > vreg_1;

vreg_1 = std::make_shared<Game_MyList >();
Game_MyList_ctor(vreg_1.get());
Game_MyList_Add(vreg_1, 2);
return;
}


template <class T1> 
System_Void Game_MyList_ctor(Game_MyList * _this)
{
std::shared_ptr< Array < T1  > > vreg_3;

vreg_3 = std::make_shared< Array <T1 > >(10); 
_this->items = vreg_3;
return;
}


template <class T1> 
System_Void Game_MyList_Add(const std::shared_ptr<Game_MyList<T1>>& _this, T1  item)
{
Game_MyList <T1>* vreg_1;
Array < T1  > * vreg_2;
std::shared_ptr<Game_MyList <T1> > vreg_3;
System_Int32 vreg_4;
std::shared_ptr<T1> vreg_5;
std::shared_ptr<Game_MyList <T1> > vreg_6;
std::shared_ptr<Game_MyList <T1> > vreg_7;
System_Int32 vreg_8;
System_Int32 vreg_9;
System_Int32 vreg_10;

vreg_1 = (_this).get();
vreg_2 = vreg_1->items.get();
vreg_3 = _this;
vreg_4 = Game_MyList_get_Count(vreg_3);
vreg_5 = item;
(*vreg_2)[vreg_4] = vreg_5; 
vreg_6 = _this;
vreg_7 = vreg_6;
vreg_8 = Game_MyList_get_Count(vreg_7);
vreg_9 = 1;
vreg_10 = vreg_8+vreg_9;
Game_MyList_set_Count(vreg_6, vreg_10);
return;
}


///---End closure code --- 
void initializeRuntime();
int main(int argc, char**argv) {
auto argsAsList = System::getArgumentsAsList(argc, argv);
initializeRuntime();
Game_App_Main();
return 0;
}
void mapLibs() {
}

void RuntimeHelpersBuildConstantTable() {
}

void buildStringTable() {
} // buildStringTable
const wchar_t _stringTable[1] = {
0
}; // _stringTable 

