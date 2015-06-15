#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
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
            return string.Format("{0} = unbox( {1})",
                Right.Name,
                AssignedTo.Name);
        }
    }
}