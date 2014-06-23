namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class AlwaysBranch : BaseOperation
    {
        public AlwaysBranch()
            : base(OperationKind.AlwaysBranch)
        {
        }
        public int JumpTo { get; set; }
    }
}