using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
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