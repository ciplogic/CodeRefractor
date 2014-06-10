#region Usings

using System.Collections.Generic;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.ConstTable
{
    public class ConstByteArrayData
    {
        public readonly byte[] Data;

        public ConstByteArrayData(byte[] values)
        {
            Data = values;
        }

        public override int GetHashCode()
        {
            if (Data == null)
                return -1;
            return Data.GetHashCode();
        }

        public override bool Equals(object item)
        {
            var other = (ConstByteArrayData) item;
            if (Data.Length != other.Data.Length)
                return false;
            for (var i = 0; i < Data.Length; i++)
            {
                if (Data[i] != other.Data[i])
                    return false;
            }
            return true;
        }

        public class EqualityComparer : IEqualityComparer<ConstByteArrayData>
        {
            public bool Equals(ConstByteArrayData x, ConstByteArrayData y)
            {
                if (x.Data.Length != y.Data.Length)
                    return false;
                for (var i = 0; i < x.Data.Length; i++)
                {
                    if (x.Data[i] != y.Data[i])
                        return false;
                }
                return true;
            }

            public int GetHashCode(ConstByteArrayData obj)
            {
                if (obj.Data == null)
                    return -1;
                var result = 121;
                for (var i = 0; i < obj.Data.Length; i++)
                    result ^= obj.Data[i]*54 + result;
                return result;
            }
        }
    }
}