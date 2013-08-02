#region Usings

using System.Collections.Generic;
using CodeRefractor.RuntimeBase.DataBase.Compacter;

#endregion

namespace CodeRefractor.RuntimeBase.DataBase
{
    public class ExpanderTransformer
    {
        private readonly Cursor _cursor = new Cursor();
        private readonly List<string> _stringTable = new List<string>();
        private readonly List<string> _elementsTable = new List<string>();

        private readonly Stack<DynNode> _stack = new Stack<DynNode>();

        private DynNode Top()
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

        private void ExistingText()
        {
            var text = ReadIdName(_stringTable);
            Top().InnerText = text;
        }

        private void CreateText()
        {
            var text = _cursor.ReadString();
            Top().InnerText = text;
        }

        private void ExistingAttrValue(string attrKey)
        {
            var attrValue = ReadIdName(_stringTable);
            Top()[attrKey] = attrValue;
        }

        private void CreateAttrValue(string attrKey)
        {
            var attrValue = _cursor.ReadString();
            _stringTable.Add(attrValue);
            Top()[attrKey] = attrValue;
        }

        private string ExistingAttrKey()
        {
            return ReadIdName(_elementsTable);
        }

        private string CreateAttrKey()
        {
            var attrKey = _cursor.ReadString();
            _elementsTable.Add(attrKey);
            return attrKey;
        }

        private void CreateExistingElement()
        {
            var name = ReadIdName(_elementsTable);
            CreateNamedElement(name);
        }

        private string ReadIdName(List<string> stringTable)
        {
            var nameId = _cursor.ReadInt();
            var name = stringTable[nameId];
            return name;
        }

        private void CreateNewElement()
        {
            var name = _cursor.ReadString();
            _stringTable.Add(name);

            CreateNamedElement(name);
        }

        private void CreateNamedElement(string name)
        {
            var creaElement = new DynNode(name);
            Top().Children.Add(creaElement);
            _stack.Push(creaElement);
        }
    }
}