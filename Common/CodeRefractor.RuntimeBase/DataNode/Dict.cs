#region Usings

using System.Collections.Generic;

#endregion

namespace CodeRefractor.RuntimeBase.DataBase
{
    internal class Dict
    {
        private readonly Dictionary<string, int> _words = new Dictionary<string, int>();

        public bool HasWord(string word)
        {
            return _words.ContainsKey(word);
        }

        public int GetWordId(string word)
        {
            int id;
            if (_words.TryGetValue(word, out id))
                return id;
            return -1;
        }

        public int Count
        {
            get { return _words.Count; }
        }

        internal void AddWord(string newText)
        {
            var id = Count;
            _words[newText] = id;
        }
    }
}