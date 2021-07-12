#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class Unboxing : LocalOperation
    {
        public LocalVariable AssignedTo;
        public IdentifierValue Right;

        public Unboxing()
            : base(OperationKind.Unbox)
        {
        }

        public override string ToString()
        {
            return $"{Right.Name} = unbox( {AssignedTo.Name})";
        }
    }
}