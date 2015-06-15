#region Uses

using System.Collections.Generic;

#endregion

namespace CodeRefractor.DataNode
{
    public class ExpanderTransformer
    {
        readonly Cursor _cursor = new Cursor();
        readonly List<string> _elementsTable = new List<string>();
        readonly Stack<DynNode> _stack = new Stack<DynNode>();
        readonly List<string> _stringTable = new List<string>();

        DynNode Top()
        {
            return _stack.Peek();
        }

        public DynNode Expand(byte[] arrayData, bool decompress)
        {
            if (decompress)
                arrayData = arrayData.Decompress();

            _cursor.ArrayData = arrayData;
            var ev = _cursor.Next();
            var root = new DynNode(_cursor.ReadString());
            _stack.Push(root);

            var attrKey = string.Empty;
            do
            {
                ev = _cursor.Next();
                switch (ev)
                {
                    case ExiLikeEvent.CreateElement:
                        CreateNewElement();
                        break;
                    case ExiLikeEvent.ExistingElement:
                        CreateExistingElement();
                        break;

                    case ExiLikeEvent.CreateAttribute:
                        attrKey = CreateAttrKey();
                        break;
                    case ExiLikeEvent.ExistingAttribute:
                        attrKey = ExistingAttrKey();
                        break;

                    case ExiLikeEvent.CreateAttributeValue:
                        CreateAttrValue(attrKey);
                        break;
                    case ExiLikeEvent.ExistingAttributeValue:
                        ExistingAttrValue(attrKey);
                        break;

                    case ExiLikeEvent.CreateText:
                        CreateText();
                        break;
                    case ExiLikeEvent.ExistingText:
                        ExistingText();
                        break;

                    case ExiLikeEvent.EndElement:
                        _stack.Pop();
                        if (_stack.Count == 0)
                            return root;
                        break;
                }
            } while (ev != ExiLikeEvent.None);
            return root;
        }

        void ExistingText()
        {
            var text = ReadIdName(_stringTable);
            Top().InnerText = text;
        }

        void CreateText()
        {
            var text = _cursor.ReadString();
            Top().InnerText = text;
        }

        void ExistingAttrValue(string attrKey)
        {
            var attrValue = ReadIdName(_stringTable);
            Top()[attrKey] = attrValue;
        }

        void CreateAttrValue(string attrKey)
        {
            var attrValue = _cursor.ReadString();
            _stringTable.Add(attrValue);
            Top()[attrKey] = attrValue;
        }

        string ExistingAttrKey()
        {
            return ReadIdName(_elementsTable);
        }

        string CreateAttrKey()
        {
            var attrKey = _cursor.ReadString();
            _elementsTable.Add(attrKey);
            return attrKey;
        }

        void CreateExistingElement()
        {
            var name = ReadIdName(_elementsTable);
            CreateNamedElement(name);
        }

        string ReadIdName(List<string> stringTable)
        {
            var nameId = _cursor.ReadInt();
            var name = stringTable[nameId];
            return name;
        }

        void CreateNewElement()
        {
            var name = _cursor.ReadString();
            _stringTable.Add(name);

            CreateNamedElement(name);
        }

        void CreateNamedElement(string name)
        {
            var creaElement = new DynNode(name);
            Top().Children.Add(creaElement);
            _stack.Push(creaElement);
        }
    }
}