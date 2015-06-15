#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class Assignment : LocalOperation
    {
        public Assignment()
            : base(OperationKind.Assignment)
        {
        }

        public LocalVariable AssignedTo { get; set; }
        public IdentifierValue Right { get; set; }

        public override string ToString()
        {
            return $"{AssignedTo.Name} = {Right.Name}";
        }
    }
}