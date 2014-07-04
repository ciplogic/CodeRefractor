#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class DerefAssignment : LocalOperation
    {
        public LocalVariable Left { get; set; }
        public LocalVariable Right { get; set; }

        public DerefAssignment()
            : base(OperationKind.DerefAssignment)
        {
        }
    }
}