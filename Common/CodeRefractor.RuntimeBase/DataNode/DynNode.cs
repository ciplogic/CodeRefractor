#region Uses

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Text;

#endregion

namespace CodeRefractor.DataNode
{
    public class DynNode : IEnumerable<DynNode>
    {
        public readonly Dictionary<string, string> Attributes;
        public readonly List<DynNode> Children;

        public DynNode(string name)
        {
            Children = new List<DynNode>();
            Attributes = new Dictionary<string, string>();
            Name = name;
        }

        public string Name { get; set; }
        public string InnerText { get; set; }

        public string this[string key]
        {
            get
            {
                if (!Attributes.ContainsKey(key))
                {
                    Attributes[key] = string.Empty;
                }
                return Attributes[key];
            }
            set { Set(key, value); }
        }

        public IEnumerator<DynNode> GetEnumerator()
        {
            return Children.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
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
            Attributes[key] = value;
            return this;
        }

        public override string ToString()
        {
            Contract.Ensures(!string.IsNullOrEmpty(Contract.Result<string>()));
            var sb = new StringBuilder();
            sb.AppendFormat("<{0}", Name);
            foreach (var atrribute in Attributes)
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

        public void Add(string name, string value)
        {
            Attributes[name] = value;
        }
    }
}