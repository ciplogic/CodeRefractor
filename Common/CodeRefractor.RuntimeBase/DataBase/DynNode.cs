using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

namespace CodeRefractor.RuntimeBase.DataBase
{
    public class DynNode : IEnumerable<DynNode>
    {
        public string Name { get; set; }

        public string InnerText { get; set; }

        public readonly List<DynNode> Children;
        public readonly Dictionary<string, string> Atrributes;

        public DynNode(string name)
        {
            Children = new List<DynNode>();
            Atrributes = new Dictionary<string, string>();
            Name = name;
        }

        public IEnumerator<DynNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        public DynNode Add(string name)
        {
            Contract.Requires(name != null);
            var child = new DynNode(name);
            Children.Add(child);
            return child;
        }

        public DynNode Get(string name)
        {
            Contract.Requires(name != null);
            foreach (var child in Children)
            {
                if (child.Name == name)
                    return child;
            }
            return null;
        }

        public DynNode Set(string key, string value)
        {
            Contract.Requires(key != null);
            Contract.Requires(value != null);
            Atrributes[key] = value;
            return this;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public override string ToString()
        {
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
            var sb = new StringBuilder();
            sb.AppendFormat("<{0}", Name);
            foreach (var atrribute in Atrributes)
            {
                sb.AppendFormat(" {0}=\"{1}\"", atrribute.Key, atrribute.Value);
            }
            if (Children.Count != 0)
            {
                sb.AppendLine(">");
                foreach (var child in Children)
                    sb.AppendLine(child.ToString());
                sb.AppendFormat("</{0}> ", Name);
            }
            else
                sb.Append("/>");
            return sb.ToString();
        }

        public string this[string key]
        {
            get { return Atrributes[key]; }
            set { Set(key, value); }
        }
    }
}