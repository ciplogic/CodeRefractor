#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Assignment : LocalOperation
    {
        public LocalVariable AssignedTo { get; set; }
        public IdentifierValue Right { get; set; }

        public Assignment()
            : base(OperationKind.Assignment)
        {
        }
    }
}