using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(double))]
    public static class DoubleImpl
    {
        [MapMethod(IsStatic = true)]
        [CppMethodBody(
            Header = "cwchar",
            Code = @" 
	            return wcstod(text->Text->Items,NULL);"
        )]
        public static double Parse(string text)
        {
            return 0;
        }


         [MapMethod(IsStatic=false)]
        [CppMethodBody(
            Header = "cwchar",
            Code =
@" 
	wchar_t buffer [26];
    int cx;
    cx = swprintf ( buffer, L""%llf\0"", value); // is passed as reference
    auto result = std::make_shared<System_String>();
	auto text = std::make_shared<Array < System_Char >>(cx, buffer);
	result->Text =  text;
	return result;"
            )]
        public static string ToString(double value)
        {
            return null;
        }

        
    }
}