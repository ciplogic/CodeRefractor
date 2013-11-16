using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using CodeRefractor.RuntimeBase;

namespace CodeRefractor.CompilerBackend.Linker
{

    class FieldNameTable
    {
        readonly Dictionary<string, string> InvalidNames = new Dictionary<string, string>();
		int CountedField;

		public static FieldNameTable Instance { get { return StaticInstance; } }

        static readonly FieldNameTable StaticInstance = new FieldNameTable();

        public string GetFieldName(string name)
        {
			if (!ContainsInvalidCharacters (name))
				return name;
			string result;
			if (InvalidNames.TryGetValue (name, out result))
				return result;
			var formattedName = string.Format ("AutoNamed_{0}", CountedField);
			CountedField++;
			InvalidNames [name] = formattedName;
			return formattedName;
        }

        public static bool ContainsInvalidCharacters(string text)
        {
			var notToFind = new []{ "<", ">" };
            var count = notToFind.Count (text.Contains);
            return count!=0;
        }
    }
    
}