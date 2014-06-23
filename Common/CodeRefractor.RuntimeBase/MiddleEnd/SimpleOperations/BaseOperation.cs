namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class BaseOperation
    {
        public BaseOperation(OperationKind kind)
        {
            Kind = kind;
        }

        public OperationKind Kind { get; private set; }

        public BaseOperation Clone()
        {
            throw new System.NotImplementedException();
        }
    }
}