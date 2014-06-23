using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.Methods
{
    public class Return : BaseOperation
    {
        public Return()
            : base(OperationKind.Return)
        {
        }

        public IdentifierValue Returning { get; set; }
    }
}