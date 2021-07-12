#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class RefArrayItemAssignment : LocalOperation
    {
        public RefArrayItemAssignment()
            : base(OperationKind.AddressOfArrayItem)
        {
        }

        public LocalVariable ArrayVar { get; set; }
        public IdentifierValue Index { get; set; }
        public LocalVariable Left { get; set; }

        public override string ToString()
        {
            return $"{ArrayVar.Name} = & ({ArrayVar}[{Index}])";
        }
    }
}