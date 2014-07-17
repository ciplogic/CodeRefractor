using System;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof (string))]
    public static class StringImpl
    {
        [MapMethod]
        public static string Substring(string _this, int startIndex)
        {
            var result = new string(new char[3]);
            return result;
        }

        [MapMethod(IsStatic = true)]
        public static string Concat(string s1, string s2)
        {
            var s1ch = s1.ToCharArray();
            var s2ch = s2.ToCharArray();

            var resultCh = new Char[s1ch.Length + s2ch.Length];
            for (var i = 0; i < s1ch.Length; i++)
            {
                resultCh[i] = s1ch[i];
            }
            var s1Len = s1ch.Length;
            for (var i = 0; i < s2ch.Length; i++)
            {
                resultCh[i+s1Len] = s2ch[i];
            }
            return new string(resultCh);
        }
    }
}