using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof(double))]
    public static class DoubleImpl
    {
        [MapMethod(IsStatic = true)]
        [CppMethodBody(Code = "return 0.0;")]
        public static double Parse(string text)
        {
            return 0;
        }
    }
}