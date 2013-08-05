#region Usings

using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
{
    public class BinaryOperator : Operator
    {
        public BinaryOperator(string name) : base(name)
        {
        }

        public override Type ComputedType()
        {
            var leftType = Left.ComputedType();
            var rightType = Right.ComputedType();
            return leftType ?? rightType;
        }

        public IdentifierValue Left { get; set; }
        public IdentifierValue Right { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", Left.Name, Name, Right.Name);
        }
    }
}