#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Operators;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
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

        public TypeDescription ComputedType()
        {
            var leftType = Left.ComputedType();
            var rightType = Right.ComputedType();
            return leftType ?? rightType;
        }

        public IdentifierValue Left { get; set; }
        public IdentifierValue Right { get; set; }

        public override string ToString()
        {
            return string.Format("{3} = {0} {1} {2}", Left.Name, Name, Right.Name, AssignedTo.Name);
        }
    }
}