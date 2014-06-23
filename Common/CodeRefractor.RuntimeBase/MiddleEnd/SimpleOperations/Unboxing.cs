using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Unboxing : LocalOperation
    {
        public IdentifierValue Right;
        public LocalVariable AssignedTo;

        public Unboxing()
            : base(OperationKind.Unbox)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = unbox( {1})",
                Right.Name,
                AssignedTo.Name);
        }
    }
}