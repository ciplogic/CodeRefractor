#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class DerefAssignment : LocalOperation
    {
        public DerefAssignment()
            : base(OperationKind.DerefAssignment)
        {
        }

        public LocalVariable Left { get; set; }
        public LocalVariable Right { get; set; }
    }
}