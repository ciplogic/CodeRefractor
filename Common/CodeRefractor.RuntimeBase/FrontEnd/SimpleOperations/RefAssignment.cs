#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class RefAssignment : LocalOperation
    {
        public LocalVariable Left;
        public LocalVariable Right;

        public RefAssignment()
            : base(OperationKind.RefAssignment)
        {
        }

        public override string ToString()
        {
            return $"{Left.Name} = {Right}";
        }
    }
}