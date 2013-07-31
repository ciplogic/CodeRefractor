namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
{
    public class BranchOperator : Operator
    {
        public BranchOperator(string name)
            : base(name)
        {
        }

        public int JumpTo { get; set; }
        public IdentifierValue CompareValue { get; set; }
        public IdentifierValue SecondValue { get; set; }
    }
}