using System;
using System.Globalization;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [ExtensionsImplementation(typeof (string))]
    public static class StringImpl
    {
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

            var resultCh = new Char[s1ch.Length + s2ch.Length+5]; //Need a proper fix for this Look at BoxingTest
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
            var startIndex = FindTrimStartIndex(_this, trimChars);

            return Substring(_this, startIndex);
        }

        [MapMethod]
        public static object TrimEnd(string _this, params char[] trimChars)
        {
            var length = FindTrimEndLength(_this, trimChars);

            return Substring(_this, 0, length);
        }

        [MapMethod]
        public static object Trim(string _this, params char[] trimChars)
        {
            int startIndex = FindTrimStartIndex(_this, trimChars),
                length = FindTrimEndLength(_this, trimChars);

            if (length == 0) // in case the string gets completely trimmed, the length - startIndex will be invalid.
            {
                return "";
            }
            
            return Substring(_this, startIndex, length - startIndex);
        }

        [MapMethod]
        public static int IndexOf(string _this, char c)
        {
            return IndexOf(_this, c, 0, _this.Length);
        }

        [MapMethod]
        public static int IndexOf(string _this, char c, int startIndex)
        {
            return IndexOf(_this, c, startIndex, _this.Length - startIndex);
        }

        [MapMethod]
        public static int IndexOf(string _this, char c, int startIndex, int count)
        {
            if (count <= 0)
            {
                // FIXME: throw ArgumentOutOfRangeException
                Console.WriteLine("Count must be positive as a parameter for IndexOf. Returning -1.");

                return -1;
            }

            if (count + startIndex > _this.Length)
            {
                // FIXME: throw ArgumentOutOfRangeException
                Console.WriteLine("Count must be inside the string boundary. Returning -1.");

                return -1;
            }

            if (startIndex < 0 || startIndex >= _this.Length)
            {
                // FIXME: throw ArgumentOutOfRangeException
                Console.WriteLine("Start index must be inside the string boundary. Returning -1.");

                return -1;
            }

            for (int i = 0; i < count; i++)
            {
                if (_this[startIndex + i] != c) // are the characters the same?
                {
                    goto nextChar;
                }

                return startIndex + i;

            nextChar: { }
            }

            return -1;
        }

        [MapMethod]
        public static int IndexOf(string _this, string str, int startIndex, int count)
        {
            return IndexOf(_this, str, startIndex, count, StringComparison.Ordinal);
        }

        [MapMethod]
        public static int IndexOf(string _this, string str, int startIndex, StringComparison stringComparison)
        {
            return IndexOf(_this, str, startIndex, _this.Length - startIndex, stringComparison);
        }

        [MapMethod]
        public static int IndexOf(string _this, string str, int startIndex)
        {
            return IndexOf(_this, str, startIndex, _this.Length - startIndex, StringComparison.Ordinal);
        }

        [MapMethod]
        public static int IndexOf(string _this, string str, StringComparison stringComparison)
        {
            return IndexOf(_this, str, 0, _this.Length, stringComparison);
        }

        [MapMethod]
        public static int IndexOf(string _this, string str)
        {
            return IndexOf(_this, str, 0, _this.Length, StringComparison.Ordinal);
        }

        [MapMethod]
        public static int IndexOf(string _this, string str, int startIndex, int count, StringComparison comparisonType)
        {
            if (comparisonType != StringComparison.Ordinal)
            {
                // FIXME: throw NotImplementedException
                Console.WriteLine("Only StringComparison.Ordinal is supported for IndexOf. Returning -1.");

                return -1;
            }

            if (count <= 0)
            {
                // FIXME: throw ArgumentOutOfRangeException
                Console.WriteLine("Count must be positive as a parameter for IndexOf. Returning -1.");

                return -1;
            }

            if (count + startIndex > _this.Length)
            {
                // FIXME: throw ArgumentOutOfRangeException
                Console.WriteLine("Count must be inside the string boundary. Returning -1.");

                return -1;
            }

            if (startIndex < 0 || startIndex >= _this.Length)
            {
                // FIXME: throw ArgumentOutOfRangeException
                Console.WriteLine("Start index must be inside the string boundary. Returning -1.");

                return -1;
            }

            for (int i = 0; i < count; i++)
            {
                for (int j = 0; j < str.Length; j++)
                {
                    if (startIndex + i + j >= _this.Length) // no reason to overflow.
                    {
                        return -1;
                    }

                    if (_this[startIndex + i + j] != str[j]) // are the characters the same?
                    {
                        goto nextChar;
                    }

                    if (i + j >= count) // we ran out of searching characters.
                    {
                        return -1;
                    }
                }

                return startIndex + i;

                nextChar: {} 
            }

            return -1;
        }

        [MapMethod]
        public static object StartsWith(string _this, string searchedString, bool ignoreCase, CultureInfo cultureInfo)
        {
            if (cultureInfo != null)
            {
                // FIXME: throw NotImplementedException
                Console.WriteLine("CultureInfo is not supported.");

                return false;
            }

            StringComparison comparison = ignoreCase ? StringComparison.OrdinalIgnoreCase : StringComparison.Ordinal;

            return StartsWith(_this, searchedString, comparison);
        }

        [MapMethod]
        public static object StartsWith(string _this, string searchedString)
        {
            return StartsWith(_this, searchedString, StringComparison.Ordinal);
        }

        [MapMethod]
        public static object StartsWith(string _this, string searchedString, StringComparison stringComparison)
        {
            if (searchedString.Length > _this.Length)
            {
                // FIXME: throw ArgumentOutOfRangeException
                Console.WriteLine("Searched string can't be longer that the actual string.");

                return false;
            }

            if (stringComparison != StringComparison.Ordinal)
            {
                // FIXME: throw NotImplementedException
                Console.WriteLine("Comparisons that are not Ordinal are not supported.");

                return false;
            }

            for (int i = 0; i < searchedString.Length; i++)
            {
                if (searchedString[i] != _this[i])
                {
                    return false;
                }
            }

            return true;
        }
        
        private static int FindTrimStartIndex(string _this, char[] trimChars)
        {
            // FIXME: default, these should be Unicode whitespace.
            // FIXME: these should be static, currently this is a WA for a compiler limitation.
            char[] defaultTrimChars = { ' ', '\n', '\t' };

            int index = 0, i;
            char c;
            int foundTrimChar;

            if (trimChars.Length == 0)
            {
                trimChars = defaultTrimChars;
            }

            if (_this.Length == 0)
            {
                return 0;
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
            int startIndex = index - 1 + foundTrimChar;
            return startIndex;
        }

        private static int FindTrimEndLength(string _this, char[] trimChars)
        {
            // FIXME: default, these should be Unicode whitespace.
            // FIXME: these should be static, currently this is a WA for a compiler limitation.
            char[] defaultTrimChars = { ' ', '\n', '\t' };

            if (trimChars.Length == 0)
            {
                trimChars = defaultTrimChars;
            }

            if (_this.Length == 0)
            {
                return 0;
            }

            int index = _this.Length - 1,
                foundTrimChar,
                i;

            do
            {
                char c = _this[index];
                foundTrimChar = 0;

                for (i = 0; i < trimChars.Length; i++)
                {
                    if (trimChars[i] == c)
                    {
                        foundTrimChar = 1;
                        break;
                    }
                }

                index--;
            } while (foundTrimChar == 1 && index >= 0);

            // This index is shifted by + 2, since it starts from Length - 1, and since it's
            // part of a Do/While
            // If the string ended, and we were still finding chars (string was made out of only trim characters),
            // adjust the length to that.
            int length = index + 2 - foundTrimChar;
            return length;
        }

        [MapMethod(IsStatic = false)]
        public static string ToString(string value)
        {
            return value;
        }
    }
}