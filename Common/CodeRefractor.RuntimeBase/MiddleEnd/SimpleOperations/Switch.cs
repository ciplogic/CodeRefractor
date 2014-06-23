namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Switch : BaseOperation
    {
        public Switch()
            : base(OperationKind.Switch)
        {
        }
        public int[] Jumps { get; set; }
    }
}