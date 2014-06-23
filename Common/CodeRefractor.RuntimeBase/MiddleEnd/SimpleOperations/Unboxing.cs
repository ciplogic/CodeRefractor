using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Unboxing : BaseOperation
    {
        public IdentifierValue Value;
        public LocalVariable AssignedTo;

        public Unboxing()
            : base(OperationKind.Unbox)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = unbox( {1})",
                Value.Name,
                AssignedTo.Name);
        }
    }
}