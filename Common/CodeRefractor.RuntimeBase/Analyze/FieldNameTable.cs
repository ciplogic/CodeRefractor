#region Usings

using System.Collections.Generic;
using System.Linq;

#endregion

namespace CodeRefractor.RuntimeBase.Analyze
{
    public class FieldNameTable
    {
        private readonly Dictionary<string, string> _invalidNames = new Dictionary<string, string>();
        private int _countedField;

        public static FieldNameTable Instance
        {
            get { return StaticInstance; }
        }

        private static readonly FieldNameTable StaticInstance = new FieldNameTable();

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