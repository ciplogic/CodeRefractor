#region Uses

using System.Collections.Generic;
using System.Linq;

#endregion

namespace CodeRefractor.Analyze
{
    public class FieldNameTable
    {
        readonly Dictionary<string, string> _invalidNames = new Dictionary<string, string>();
        int _countedField;
        public static FieldNameTable Instance { get; } = new FieldNameTable();

        public string GetFieldName(string name)
        {
            if (!ContainsInvalidCharacters(name))
                return name;
            string result;
            if (_invalidNames.TryGetValue(name, out result))
                return result;
            var formattedName = string.Format("AutoNamed_{0}", _countedField);
            _countedField++;
            _invalidNames[name] = formattedName;
            return formattedName;
        }

        public static bool ContainsInvalidCharacters(string text)
        {
            var notToFind = new[] {"<", ">"};
            var count = notToFind.Count(text.Contains);
            return count != 0;
        }
    }
}