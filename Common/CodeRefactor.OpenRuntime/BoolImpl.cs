using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(bool))]
    public static class BoolImpl
    {
        [MapMethod(IsStatic = true)]
        [CppMethodBody(
            Header = "cwchar",
            Code = @"
bool vOut = vIn && wcsicmp(text->Text->Items,L""True"")==0;
	            return vOut;"
            )]
        public static int Parse(string text)
        {
            return 0;
        }

        [MapMethod(IsStatic = false)]
        public static string ToString(bool value)
        {
            if (value)
                return "True";

            return "False";
        }
    }
}