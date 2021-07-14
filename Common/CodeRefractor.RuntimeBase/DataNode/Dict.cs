#region Uses

using System.Collections.Generic;

#endregion

namespace CodeRefractor.DataNode
{
    internal class Dict
    {
        private readonly Dictionary<string, int> _words = new Dictionary<string, int>();

        public int Count => _words.Count;

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

        internal void AddWord(string newText)
        {
            var id = Count;
            _words[newText] = id;
        }
    }
}