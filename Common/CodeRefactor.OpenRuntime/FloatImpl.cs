using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(float))]
    public static class FloatImpl
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

        [MapMethod(IsStatic = false)]
        [CppMethodBody(
            Header = "cwchar",
            Code =
                @" 
	wchar_t buffer [15];
    int cx;
    cx = swprintf ( buffer, L""%.6f"", value); // is passed as reference

	// Remove trailing 0 (after .)
	if (wcschr(buffer, '.') != NULL)
	{
		while (cx > 0 && buffer[cx - 1] == '0')
			buffer[--cx] = '\0';
	}

	if (cx > 0 && buffer[cx - 1] == '.')
		buffer[--cx] = '\0';

    auto result = std::make_shared<System_String>();
	auto text = std::make_shared<Array < System_Char >>(cx, buffer);
	result->Text =  text;
	return result;"
            )]
        public static string ToString(float value)
        {
            return null;
        }
    }
}