using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(double))]
    public static class DoubleImpl
    {
        [MapMethod(IsStatic = true)]
        [CppMethodBody(
            Header = "stdio.h",
            Code = "System_Double result; "+
    "sscanf (text->Text->Items, L\"%lf\", &result); " +
	"return result;"
)]
        public static double Parse(string text)
        {
            return 0;
        }
    }
}