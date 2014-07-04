#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class ClassCasting : LocalOperation
    {
        public IdentifierValue Value { get; set; }
        public LocalVariable AssignedTo { get; set; }

        public ClassCasting()
            : base(OperationKind.CastClass)
        {
        }
    }
}