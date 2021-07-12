#region Uses

using CodeRefractor.Analyze;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Operators
{
    public class BinaryOperator : OperatorBase
    {
        public BinaryOperator() : base("", OperationKind.BinaryOperator)
        {
        }

        public BinaryOperator(string name)
            : base(name, OperationKind.BinaryOperator)
        {
        }

        public IdentifierValue Left { get; set; }
        public IdentifierValue Right { get; set; }

        public TypeDescription ComputedType()
        {
            var leftType = Left.ComputedType();
            var rightType = Right.ComputedType();
            return leftType ?? rightType;
        }

        public override string ToString()
        {
            return string.Format("{3} = {0} {1} {2}", Left.Name, Name, Right.Name, AssignedTo.Name);
        }
    }
}