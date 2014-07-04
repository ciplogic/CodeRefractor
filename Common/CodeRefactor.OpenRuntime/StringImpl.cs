using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof (double))]
    public static class StringImpl
    {
        [MapMethod]
        public static string Substring(string _this, int startIndex)
        {
            var result = new string(new char[3]);
            return result;
        }
    }
}