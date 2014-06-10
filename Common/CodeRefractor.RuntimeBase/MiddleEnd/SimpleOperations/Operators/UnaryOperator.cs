#region Usings

using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
{
    public class UnaryOperator : OperatorBase
    {
        public UnaryOperator(string name)
            : base(name)
        {
        }

        public IdentifierValue Left { get; set; }

        public override string ToString()
        {
            return String.Format("{0} = ({2}){1}", AssignedTo, Left, Name);
        }
    }
}