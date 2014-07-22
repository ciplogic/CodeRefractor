#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Casts
{
    public class Boxing : LocalOperation
    {
        public IdentifierValue Right { get; set; }
        public LocalVariable AssignedTo { get; set; }

        public Boxing()
            : base(OperationKind.Box)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = box({1})",
                Right.Name,
                AssignedTo.Name);
        }
    }
}