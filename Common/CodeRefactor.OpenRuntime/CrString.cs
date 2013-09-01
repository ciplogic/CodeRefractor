#region Usings

using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (string),
        Code=@"
public:
    ")]
    public class CrString
    {
        public int Lengh;
        public char[] Text;

        public unsafe CrString(byte* data)
        {
            var len = StrLen(data);
            Text = new char[len + 1];
            for (var i = 0; i < len; i++)
                Text[i] = (char) data[i];
            Text[len] = '\0';
            Lengh = len;
        }

        public CrString(char[] value)
        {
            var length = value.Length;
            Lengh = length - 1;
            Text = new char[length];
            for (var i = 0; i <= length; i++)
                Text[i] = value[i];
        }

        private static unsafe int StrLen(byte* data)
        {
            var result = 0;
            while (*data != 0)
            {
                result++;
                data++;
            }
            return result;
        }
    }
}