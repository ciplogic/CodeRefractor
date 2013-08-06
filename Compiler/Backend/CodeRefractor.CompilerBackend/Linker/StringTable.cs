using System;
using System.Collections.Generic;
using System.Text;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.CompilerBackend.Linker
{
    class StringTable
    {
        private readonly Dictionary<string, int> _stringsDictionary = new Dictionary<string, int>();
        private readonly List<string> _table = new List<string>();
        public int GetStringId(string text)
        {
            int result;
            if (!_stringsDictionary.TryGetValue(text, out result))
            {
                result = _table.Count;
                _stringsDictionary[text] = result;
                _table.Add(text);
            }
            return result;
        }

        static short[] TextData(string text)
        {
            var result = new short[text.Length + 1];
            for (var i = 0; i < text.Length; i++)
            {
                result[i] = (short)text[i];
            }
            result[text.Length] = 0;
            return result;
        }

        public string BuildStringTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine("void buildStringTable() {");

            var stringDataBuilder = new List<string>();

            var jump = 0;
            foreach (var strItem in _table)
            {
                sb.AppendFormat("_AddJumpAndLength({0}, {1});", jump, strItem.Length)
                    .AppendLine();
                var itemTextData = TextData(strItem);
                AddTextToStringTable(stringDataBuilder, itemTextData, strItem);

                jump += strItem.Length + 1;
            }


            sb.AppendLine("} // buildStringTable");

            var stringTableContent = String.Join(", "+Environment.NewLine, stringDataBuilder);
            sb.AppendFormat("const wchar_t _stringTable[{0}] = {{", jump).AppendLine();
            sb.AppendLine(stringTableContent);
            sb.AppendLine("}; // _stringTable ");

            return sb.ToString();
        }

        private static void AddTextToStringTable(List<string> stringDataBuilder, short[] itemTextData, string strItem)
        {
            var itemsText = String.Join(", ", itemTextData);
            var commentedString = String.Format("/* {0} */", strItem.ToEscapedString());
            var resultItem = String.Format("{0} {1}", itemsText, commentedString);
            stringDataBuilder.Add(resultItem);
        }
    }
}