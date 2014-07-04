using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation]
    public static class CrBasicTypesImplementations
    {
        [MapMethod(typeof(int), IsStatic = true)]
        public static int Parse(string text)
        {
            return 0;
        }
        [MapMethod(typeof(double), Name="Parse",IsStatic = true)]
        [CppMethodBody(Code = "return 0.0;")]
        public static double ParseDouble(string text)
        {
            return 0;
        }
        [MapMethod]
        public static string Substring(string _this, int startIndex)
        {
            var result = new string(new char[3]);
            return result;
        }
    }
}