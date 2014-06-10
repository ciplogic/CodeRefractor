#region Usings

using System;
using CodeRefractor.RuntimeBase.Analyze;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
{
    public class BranchOperator : OperatorBase
    {
        public BranchOperator(string name)
            : base(name)
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