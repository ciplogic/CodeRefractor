namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class AlwaysBranch : LocalOperation
    {
        public AlwaysBranch()
            : base(OperationKind.AlwaysBranch)
        {
        }
        public int JumpTo { get; set; }
    }
}