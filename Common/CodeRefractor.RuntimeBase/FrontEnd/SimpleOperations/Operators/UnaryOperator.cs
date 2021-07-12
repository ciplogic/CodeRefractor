#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Operators
{
    public class UnaryOperator : OperatorBase
    {
        public UnaryOperator()
            : base("", OperationKind.UnaryOperator)
        {
        }

        public UnaryOperator(string name)
            : base(name, OperationKind.UnaryOperator)
        {
        }

        public IdentifierValue Left { get; set; }

        public override string ToString()
        {
            return string.Format("{0} = ({2}){1}", AssignedTo, Left, Name);
        }
    }
}