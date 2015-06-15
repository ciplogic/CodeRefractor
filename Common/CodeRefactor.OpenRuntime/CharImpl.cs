using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(char))]
    public static class CharImpl
    {
        [MapMethod(IsStatic = true)]
        [CppMethodBody(
            Header = "cwchar",
            Code = @"
wchar_t vOut = (wchar_t)wcstol(text->Text->Items,NULL,10);
	            return vOut;"
            )]
        public static char Parse(string text)
        {
            return (char)0;
        }

        [MapMethod(IsStatic = false)]
        public static string ToString(char value)
        {
            return new string(new[]{value});
        }
    }
}