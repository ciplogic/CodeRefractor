using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
<<<<<<< HEAD
        public OperationKind Kind{get { return Value.Kind; }}
=======
        public OperationKind Kind;
>>>>>>> master
        public BaseOperation Value;

        public T Get<T>() where T:BaseOperation
        {
            if (Value == null)
                return default(T);
            return (T) Value;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Kind, Value.ToString() ?? "");
        }

        public LocalOperation Clone()
        {
            return new LocalOperation
            {
<<<<<<< HEAD
=======
                Kind = Kind,
>>>>>>> master
                Value = Value.Clone()
            };
        }
    }
}