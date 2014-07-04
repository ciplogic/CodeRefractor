namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class AlwaysBranch : LocalOperation
    {
        public AlwaysBranch()
            : base(OperationKind.AlwaysBranch)
        {
        }

        public int JumpTo { get; set; }

        public override string ToString()
        {
            return string.Format("goto {0}:", JumpTo);
        }
    }
}