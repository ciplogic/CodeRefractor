namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Label : LocalOperation
    {
        public Label()
            : base(OperationKind.Label)
        {
        }
        public int JumpTo { get; set; }
    }
}