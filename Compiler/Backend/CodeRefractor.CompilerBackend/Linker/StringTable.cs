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
                string escapedString = strItem.ToEscapedString();
                sb.AppendFormat("_stringLength.push_back({0});", escapedString.Length)
                    .AppendLine();
                sb.AppendFormat("_stringTable.push_back(L{0});", escapedString)
                    .AppendLine();
            }
            sb.AppendLine("}");
            return sb.ToString();
        }
    }
}