//
// Created by cipri on 7/12/2021.
//


#include <vector>
#include <memory>

#include "System_Primitives.h"

struct System_String;

namespace {
    std::vector<std::shared_ptr<System_String> > _stringJumps;
    static std::vector<System_Byte*> _constTables;
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
    result->Text = value;
    _stringJumps.push_back(result);
}

System_Byte* RuntimeHelpers_GetBytes(int id)
{
    return _constTables[id];
}

void AddConstantByteArray(System_Byte* data)
{
    _constTables.push_back(data);
}