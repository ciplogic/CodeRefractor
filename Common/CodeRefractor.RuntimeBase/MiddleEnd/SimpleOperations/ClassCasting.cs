using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class ClassCasting : LocalOperation
    {
        public IdentifierValue Value;
        public LocalVariable AssignedTo;

        public ClassCasting()
            : base(OperationKind.CastClass)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = ({1})( {2})",
                Value.Name,
                AssignedTo.FixedType,
                AssignedTo.Name);
        }
    }
}
