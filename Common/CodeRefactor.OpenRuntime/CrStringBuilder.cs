using System;
using System.Text;
using CodeRefractor.RuntimeBase;

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof(StringBuilder))]
    public class CrStringBuilder
    {
        char[] _data = new char[10];
        private int _writtenLength;
        [CilMethod]
        public void Append(string value)
        {
            ExpectAddLength(value.Length);
            CopyStrToEnd(value);
        }

        private void CopyStrToEnd(string str)
        {
            var startLength = _writtenLength;
            for(var i=0;i<str.Length;i++)
            {
                _data[startLength + i] = str[i];
            }
            _writtenLength += str.Length;
        }

        private void ExpectAddLength(int length)
        {
            var newLength = _writtenLength + length;
            if (newLength > _data.Length)
                Array.Resize(ref _data, newLength*3/2);
        }

        [CilMethod]
        public override string ToString()
        {
            return new string(_data, 0, _writtenLength);
        }
    }
}