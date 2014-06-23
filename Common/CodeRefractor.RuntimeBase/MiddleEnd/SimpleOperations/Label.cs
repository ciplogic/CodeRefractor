namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Label : BaseOperation
    {
        public Label()
            : base(OperationKind.Label)
        {
        }
        public int JumpTo { get; set; }
    }
}