using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{

    [ExtensionsImplementation(typeof(int))]
    public static class Int32Impl
    {
        [MapMethod(IsStatic = true)]
        public static int Parse(string text)
        {
            return 0;
        }
    }
}