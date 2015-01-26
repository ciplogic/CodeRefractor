#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Casts
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