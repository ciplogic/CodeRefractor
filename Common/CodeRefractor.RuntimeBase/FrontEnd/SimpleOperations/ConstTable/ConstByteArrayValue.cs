#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.ConstTable
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