#region Usings

using System.Collections.Generic;
using System.Text;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable
{
    public class ConstByteArrayList
    {
        public Dictionary<ConstByteArrayData, int> Items =
            new Dictionary<ConstByteArrayData, int>(new ConstByteArrayData.EqualityComparer());

        public List<ConstByteArrayData> ItemList = new List<ConstByteArrayData>();
        private static readonly ConstByteArrayList StaticInstance = new ConstByteArrayList();

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

        public static ConstByteArrayList Instance
        {
            get { return StaticInstance; }
        }

        public static string BuildConstantTable()
        {
            var sb = new StringBuilder();
            sb.AppendLine("void RuntimeHelpersBuildConstantTable() {");
            foreach (var item in Instance.ItemList)
            {
                var rightArray = item.Data;
                var rightArrayItems = string.Join(", ", rightArray);

                sb.AppendFormat("AddConstantByteArray(new byte[{0}] {{ {1} }} );",
                                rightArray.Length,
                                rightArrayItems);
            }
            sb.AppendLine("}");

            return sb.ToString();
        }
    }
}