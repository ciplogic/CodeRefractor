#region Uses

using System;
using System.Collections.Generic;
using System.Text;
using CodeRefractor.CodeWriter.Output;

#endregion

namespace CodeRefractor.CodeWriter.Linker
{
    internal class StringTableData
    {
        private readonly Dictionary<string, int> _stringsDictionary = new Dictionary<string, int>();
        public List<string> Table { get; } = new List<string>();

        public int GetStringId(string text)
        {
            int result;
            if (_stringsDictionary.TryGetValue(text, out result)) return result;
            result = Table.Count;
            _stringsDictionary[text] = result;
            Table.Add(text);
            return result;
        }
        
    }
    public class StringTable
    {
        private readonly StringTableData _stringTableData = new StringTableData();

        public int GetStringId(string text)
        {
            return _stringTableData.GetStringId(text);
        }

        private static short[] TextData(string text)
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
            sb.BlankLine()
                .Append("System_Void buildStringTable()")
                .BracketOpen();

            var stringDataBuilder = new List<string>();

            var jump = 0;
            foreach (var strItem in _stringTableData.Table)
            {
                sb.AppendFormat("_AddJumpAndLength({0}, {1});\n", jump, strItem.Length);
                var itemTextData = TextData(strItem);
                AddTextToStringTable(stringDataBuilder, itemTextData, strItem);

                jump += strItem.Length + 1;
            }


            sb.BracketClose(true)
                .Append(" // buildStringTable\n");

            var stringTableContent = string.Join(", " + Environment.NewLine, stringDataBuilder);
            var length = jump == 0 ? 1 : jump;
            sb.BlankLine()
                .AppendFormat("const System_Char _stringTable[{0}] =", length)
                .BracketOpen();
            sb.Append(jump == 0 ? "0" : stringTableContent);
            sb.BracketClose(true)
                .Append("; // _stringTable\n");

            return sb.ToString();
        }

        private static void AddTextToStringTable(List<string> stringDataBuilder, short[] itemTextData, string strItem)
        {
            var itemsText = string.Join(", ", itemTextData);
            var commentedString = $"/* {strItem.ToEscapedString()} */";
            var resultItem = $"{itemsText} {commentedString}";
            stringDataBuilder.Add(resultItem);
        }
    }
}