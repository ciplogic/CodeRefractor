namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
        public OperationKind Kind;
        public object Value;

        public T Get<T>()
        {
            if (Value == null)
                return default(T);
            return (T) Value;
        }

        public override string ToString()
        {
            return string.Format("{0}: {1}", Kind, Value ?? "");
        }

        public LocalOperation Clone()
        {
            var value = (IClonableOperation)Value;
            return new LocalOperation
                       {
                           Kind = Kind,
                           Value = value.Clone()
                       };
        }
    }
}