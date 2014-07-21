using System;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof (string))]
    public static class StringImpl
    {
        // FIXME: default, these should be Unicode whitespace.
        private static char[] defaultTrimChars = {' ', '\n', '\t'};

        [MapMethod]
        public static string Substring(string _this, int startIndex)
        {
            var length = _this.Length;
            var resultLen = length - startIndex;
            var resultChars = new char[resultLen];
            var originalChars = _this.ToCharArray();
            for (var i = 0; i < resultLen; i++)
                resultChars[i] = originalChars[i + startIndex];
            var result = new string(resultChars);
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


        [MapMethod]
        public static char[] ToCharArray(CrString _this)
        {
            var length = _this.Length;
            var result = new char[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = _this.Text[i];
            }
            return result;
        }

        [MapMethod]
        public static object Substring(string _this, int startIndex, int length)
        {
            var resultChars = new char[length];

            var originalChars = _this.ToCharArray();
            for (var i = 0; i < length; i++)
                resultChars[i] = originalChars[i + startIndex];
            var result = new string(resultChars);
            return result;
        }

        [MapMethod]
        public static object TrimStart(string _this, params char[] trimChars)
        {
            int index = 0, i;
            char c;
            int foundTrimChar;
            
            if (trimChars.Length == 0)
            {
                trimChars = defaultTrimChars;
            }

            do
            {
                c = _this[index];
                foundTrimChar = 0;

                for (i = 0; i < trimChars.Length; i++)
                {
                    if (trimChars[i] == c)
                    {
                        foundTrimChar = 1;
                        break;
                    }
                }

                index++;
            } while (foundTrimChar == 1 && index < _this.Length);

            // since it's a do/while, the index gets increased by one, even when the trim char check failed.
            // in case the string length was traversed, but we found all the chars, we need to make sure we don't keep
            // the last character.
            return Substring(_this, index - 1 + foundTrimChar);
        }
    }
}