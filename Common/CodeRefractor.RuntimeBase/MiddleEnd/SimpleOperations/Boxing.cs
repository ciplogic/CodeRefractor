using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class Comment : BaseOperation
    {
        public Comment()
            : base(OperationKind.Comment)
        {
        }

        public string Message { get; set; }
    }
    public class Boxing : BaseOperation
    {
        public IdentifierValue Value;
        public LocalVariable AssignedTo;

        public Boxing()
            : base(OperationKind.Box)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = box( {1})", 
                Value.Name, 
                AssignedTo.Name);
        }
    }
}