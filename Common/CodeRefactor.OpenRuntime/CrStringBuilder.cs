using System;
using System.Text;
using CodeRefractor.Runtime.Annotations;

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (StringBuilder))]
    public class CrStringBuilder
    {
        private char[] _data = new char[10];
        private int _writtenLength;

        public CrStringBuilder Append(string value)
        {
            ExpectAddLength(value.Length);
            CopyStrToEnd(value);
            return this;
        }

        private void CopyStrToEnd(string str)
        {
            int startLength = _writtenLength;
            for (int i = 0; i < str.Length; i++)
            {
                _data[startLength + i] = str[i];
            }
            _writtenLength += str.Length;
        }

        private void ExpectAddLength(int length)
        {
            int newLength = _writtenLength + length;
            if (newLength > _data.Length)
                Array.Resize(ref _data, newLength*3/2);
        }

        public override string ToString()
        {
            return new string(_data, 0, _writtenLength);
        }
    }
}