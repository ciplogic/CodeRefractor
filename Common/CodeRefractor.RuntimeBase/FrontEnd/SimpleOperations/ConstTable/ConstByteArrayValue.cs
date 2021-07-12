#region Uses

using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.ConstTable
{
    public class ConstByteArrayValue : ConstValue
    {
        public readonly int Id;

        public ConstByteArrayValue(int id) : base(id)
        {
            Id = id;
            FixedType = new TypeDescription(typeof (byte));
            Value = ConstByteArrayList.Instance.ItemList[id];
        }
    }
}