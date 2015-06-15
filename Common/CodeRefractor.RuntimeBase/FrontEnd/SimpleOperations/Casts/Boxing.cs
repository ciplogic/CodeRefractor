#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Casts
{
    public class Boxing : LocalOperation
    {
        public Boxing()
            : base(OperationKind.Box)
        {
        }

        public IdentifierValue Right { get; set; }
        public LocalVariable AssignedTo { get; set; }

        public static bool IsUsed { get; set; }

        public override string ToString()
        {
            return $"{Right.Name} = box({AssignedTo.Name})";
        }
    }
}