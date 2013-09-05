#region Usings

using CodeRefractor.RuntimeBase;

#endregion

namespace CodeRefactor.OpenRuntime
{
    [MapType(typeof (string),
        Code= @"
	System::Char* _data;
		int Length;
		String() {
			_data = 0;
		}
		~String() {
			delete []_data;
		}
		String(char* source){
			int len = strlen(source);
			auto data = new System::Char[len+1];
			for(auto i=0; i<=len; i++)
			{
				data[i] = source[i];
			}
			Initialize(len, data);
			delete []data;
		}
		String(int len, const System::Char* data){
			Initialize(len, data);			
		}

		void Initialize(int len, const System::Char* data)
		{
			_data = new System::Char [len+1];
			Length = len;
			memcpy(_data, data, len*sizeof(System::Char));
			_data[len]=0;
		}
		
		System::Char* get(){
			return _data;
		}
    ")]
    public class CrString
    {
        public int Lengh
        {
            get { return Text.Length; }
        }
        public char[] Text;

        public unsafe CrString(byte* data)
        {
            var len = StrLen(data);
            Text = new char[len + 1];
            for (var i = 0; i < len; i++)
                Text[i] = (char) data[i];
            Text[len] = '\0';
        }

        public CrString(char[] value)
        {
            var length = value.Length;
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