namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
        public LocalOperation(OperationKind kind)
        {
            Kind = kind;
        }

        public OperationKind Kind { get; private set; }

        public virtual LocalOperation Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}