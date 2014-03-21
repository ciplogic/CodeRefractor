using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable
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