#region Uses

using System.Collections.Generic;
using System.Text;

#endregion

namespace CodeRefractor.DataNode
{
    public class MinimizeTransformer
    {
        readonly Dict _dict = new Dict();
        readonly Dict _elementsDict = new Dict();
        public readonly List<byte> Result = new List<byte>();

        public byte[] Minimize(DynNode node, bool compress)
        {
            Process(node);
            var resultArray = Result.ToArray();
            return !compress
                ? resultArray
                : resultArray.Compress();
        }

        static int Bytes(int length)
        {
            var counter = 0;
            if (length == 0)
                return 1;
            while (length > 0)
            {
                counter++;
                length /= 256;
            }
            return counter;
        }

        void PushStringById(string item, Dict dict)
        {
            var id = dict.GetWordId(item);
            PushInt(id);
        }

        void Process(DynNode node)
        {
            if (node.Name == "#text")
            {
                CreateOrUpdateItemWithText(node.InnerText, ExiLikeEvent.ExistingText, ExiLikeEvent.CreateText, _dict);
                return;
            }
            CreateOrUpdateItemWithText(node.Name, ExiLikeEvent.ExistingElement, ExiLikeEvent.CreateElement,
                _elementsDict);

            foreach (var attr in node.Attributes)
            {
                CreateOrUpdateItemWithText(attr.Key, ExiLikeEvent.ExistingAttribute, ExiLikeEvent.CreateAttribute,
                    _elementsDict);
                CreateOrUpdateItemWithText(attr.Value, ExiLikeEvent.ExistingAttributeValue,
                    ExiLikeEvent.CreateAttributeValue, _dict);
            }
            foreach (var child in node.Children)
            {
                Process(child);
            }
            PushEvent(ExiLikeEvent.EndElement);
        }

        void CreateOrUpdateItemWithText(string newText, ExiLikeEvent existingElement,
            ExiLikeEvent newElementEvent, Dict dict)
        {
            if (_dict.HasWord(newText))
            {
                PushEvent(existingElement);
                PushStringById(newText, dict);
            }
            else
            {
                PushEventString(newElementEvent, newText, dict);
            }
        }

        void PushEvent(ExiLikeEvent exi)
        {
            Result.Add((byte)(int)(exi));
        }

        static byte[] StrAsBytes(string yourString)
        {
            return Encoding.UTF8.GetBytes(yourString);
        }

        static byte[] IntAsBytes(int intValue, int byteCount)
        {
            var intBytes = new List<byte>();
            while (byteCount > 0)
            {
                intBytes.Add((byte)(intValue % 256));
                intValue /= 256;
                byteCount--;
            }
            return intBytes.ToArray();
        }

        void PushInt(int intValue)
        {
            var byteCount = Bytes(intValue * 4);
            var length = IntAsBytes(intValue * 4 + byteCount - 1, byteCount);
            PushByteArray(length);
        }

        void PushByteArray(byte[] length)
        {
            Result.AddRange(length);
        }

        void PushEventString(ExiLikeEvent exi, string newText, Dict dict)
        {
            PushEvent(exi);
            dict.AddWord(newText);
            var buffer = StrAsBytes(newText);
            PushInt(buffer.Length);
            PushByteArray(buffer);
        }
    }
}