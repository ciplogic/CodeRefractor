#region Usings

using CodeRefractor.Runtime.Annotations;

#endregion

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (string))]
    public class CrString
    {
        public char[] Text;

        public unsafe CrString(byte* data)
        {
            int len = StrLen(data);
            Text = new char[len + 1];
            for (int i = 0; i < len; i++)
                Text[i] = (char) data[i];
            Text[len] = '\0';
        }

        public CrString(char[] value)
        {
            int length = value.Length;
            Text = new char[length];
            for (int i = 0; i <= length; i++)
                Text[i] = value[i];
        }

        public CrString(char[] value, int startPos, int length)
        {
            Text = new char[length];
            for (int i = 0; i < length; i++)
                Text[i] = value[startPos+i];
        }

        public int Length => Text.Length-1;

        static unsafe int StrLen(byte* data)
        {
            int result = 0;
            while (*data != 0)
            {
                result++;
                data++;
            }
            return result;
        }

        public static string ToString(string value)
        {
            return value;
        }

    }
}