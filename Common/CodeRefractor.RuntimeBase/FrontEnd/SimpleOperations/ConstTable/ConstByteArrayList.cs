#region Uses

using System.Collections.Generic;
using System.Text;
using CodeRefractor.CodeWriter.Output;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.ConstTable
{
    public class ConstByteArrayList
    {
        public List<ConstByteArrayData> ItemList = new List<ConstByteArrayData>();

        public Dictionary<ConstByteArrayData, int> Items =
            new Dictionary<ConstByteArrayData, int>(new ConstByteArrayData.EqualityComparer());

        public static ConstByteArrayList Instance { get; } = new ConstByteArrayList();

        public static int RegisterConstant(byte[] values)
        {
            var data = new ConstByteArrayData(values);
            int resultId;
            if (Instance.Items.TryGetValue(data, out resultId))
            {
                return resultId;
            }
            var id = Instance.Items.Count;
            Instance.ItemList.Add(data);
            Instance.Items[data] = id;
            return id;
        }

        public static string BuildConstantTable()
        {
            var sb = new StringBuilder();

            sb.BlankLine()
                .Append("System_Void RuntimeHelpersBuildConstantTable()")
                .BracketOpen();

            foreach (var item in Instance.ItemList)
            {
                var rightArray = item.Data;
                var rightArrayItems = string.Join(", ", rightArray);

                sb.AppendFormat("AddConstantByteArray(new byte[{0}]);", rightArray.Length)
                    .BracketOpen()
                    .AppendFormat("{1}", rightArrayItems)
                    .BracketClose();
            }

            return sb.BracketClose()
                .ToString();
        }
    }
}