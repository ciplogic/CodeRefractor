using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(long))]
    public static class LongImpl
    {
        [MapMethod(IsStatic = true)]
        [CppMethodBody(
            Header = "cwchar",
            Code = @" 
	            return (long)wcstol(text->Text->Items,NULL,10);"
            )]
        public static long Parse(string text)
        {
            return 0;
        }
    }
}