namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class LocalOperation
    {
        public LocalOperation(OperationKind kind)
        {
            Kind = kind;
        }

        public OperationKind Kind { get; private set; }

        public LocalOperation Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}