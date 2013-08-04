using System.Collections.Generic;
using System.Text;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.CompilerBackend.Linker
{
    class StringTable
    {
        public Dictionary<string, int> StringsDictionary = new Dictionary<string, int>();
        public List<string> Table = new List<string>();
        public int GetStringId(string text)
        {
            int result;
            if (!StringsDictionary.TryGetValue(text, out result))
            {
                result = Table.Count;
                StringsDictionary[text] = result;
                Table.Add(text);
            }
            return result;
        }


        public string BuildStringTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine("void buildStringTable() {");
            foreach (var strItem in Table)
            {
                sb.AppendFormat("_stringTable.push_back(std::wstring (L{0}));", strItem.ToEscapedString());
                sb.AppendLine();
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}