namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
        public OperationKind Kind{get { return Value.Kind; }}
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
                Value = Value.Clone()
            };
        }
    }
}