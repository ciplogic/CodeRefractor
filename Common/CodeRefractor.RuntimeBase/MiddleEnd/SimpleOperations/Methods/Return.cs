#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.Methods
{
    public class Return : LocalOperation
    {
        public Return()
            : base(OperationKind.Return)
        {
        }

        public IdentifierValue Returning { get; set; }
    }
}