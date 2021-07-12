#pragma once
struct System_Object;
struct CodeRefactor_OpenRuntime_CrString;
struct SimpleAdditions_TestShapes;
struct CodeRefactor_OpenRuntime_CrConsole;
struct SimpleAdditions_Shape;
struct SimpleAdditions_Square;
struct SimpleAdditions_Cube;
struct CodeRefactor_OpenRuntime_CrMath;
struct System_Object {
int _typeId;
}
;

struct System_String : public System_Object {
System_String() {
_typeId = 11;
}

std::shared_ptr< Array < System_Char > > Text;
}
;

struct SimpleAdditions_TestShapes : public System_Object {
}
;

struct System_Console : public System_Object {
}
;

struct SimpleAdditions_Shape : public System_Object {
}
;

struct SimpleAdditions_Square : public SimpleAdditions_Shape {
System_Double side;
}
;

struct SimpleAdditions_Cube : public SimpleAdditions_Shape {
System_Double side;
}
;

struct System_Math : public System_Object {
}
;

