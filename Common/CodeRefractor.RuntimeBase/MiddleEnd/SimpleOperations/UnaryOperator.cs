namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class UnaryOperator : Operator
    {
        public UnaryOperator(string name)
            : base(name)
        {
        }

        public IdentifierValue Left { get; set; }
    }
}