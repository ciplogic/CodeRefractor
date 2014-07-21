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
    }
}