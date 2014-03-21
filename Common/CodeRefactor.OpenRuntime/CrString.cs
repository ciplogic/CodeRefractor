#region Usings

using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (string))]
    public class CrString
    {
        public int Lengh
        {
            [CilMethod]
            get { return Text.Length; }
        }
        public char[] Text;

        [CilMethod]
        public unsafe CrString(byte* data)
        {
            var len = StrLen(data);
            Text = new char[len + 1];
            for (var i = 0; i < len; i++)
                Text[i] = (char) data[i];
            Text[len] = '\0';
        }

        [CilMethod]
        public CrString(char[] value)
        {
            var length = value.Length;
            Text = new char[length];
            for (var i = 0; i <= length; i++)
                Text[i] = value[i];
        }

        [CilMethod]
        public CrString(char[] value, int startPos, int length)
        {
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

        public override string ToString()
        {
            return string.Empty;
        }
    }
}