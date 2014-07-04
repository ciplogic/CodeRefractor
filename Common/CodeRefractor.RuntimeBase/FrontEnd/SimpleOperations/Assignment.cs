#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class Assignment : LocalOperation
    {
        public LocalVariable AssignedTo { get; set; }
        public IdentifierValue Right { get; set; }

        public Assignment()
            : base(OperationKind.Assignment)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", AssignedTo.Name, Right.Name);
        }
    }
}