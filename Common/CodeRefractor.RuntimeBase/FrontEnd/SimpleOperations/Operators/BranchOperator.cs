#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.Operators
{
    public class BranchOperator : OperatorBase
    {
        public BranchOperator() : base("", OperationKind.BranchOperator)
        {
        }

        public BranchOperator(string name)
            : base(name, OperationKind.BranchOperator)
        {
        }

        public int JumpTo { get; set; }
        public IdentifierValue CompareValue { get; set; }
        public IdentifierValue SecondValue { get; set; }

        public override string ToString()
        {
            return String.Format("Branch operator {0} {2} {1}? jump label_{3}",
                CompareValue, SecondValue,
                Name,
                JumpTo.ToHex());
        }
    }
}