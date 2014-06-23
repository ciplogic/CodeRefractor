using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Boxing : LocalOperation
    {
        public IdentifierValue Right;
        public LocalVariable AssignedTo;

        public Boxing()
            : base(OperationKind.Box)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = box( {1})",
                Right.Name, 
                AssignedTo.Name);
        }
    }
}