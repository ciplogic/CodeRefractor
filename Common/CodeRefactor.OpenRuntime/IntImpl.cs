using System;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(int))]
    public static class IntImpl
    {
        [MapMethod(IsStatic = true)]
        [CppMethodBody(
            Header = "cwchar",
            Code = @" 
	            return _wtoi(text->Text->Items);"
            )]
        public static int Parse(string text)
        {
            return 0;
        }

        [MapMethod(IsStatic=false)]
        [CppMethodBody(
            Header = "cwchar",
            Code =
@" 
	wchar_t buffer [15];
    int cx;
    cx = swprintf ( buffer, L""%d\0"", value); // is passed as reference
    auto result = std::make_shared<System_String>();
	auto text = std::make_shared<Array < System_Char >>(cx, buffer);
	result->Text =  text;
	return result;"
            )]
        public static string ToString(int value)
        {
            return null;
        }
    }
}