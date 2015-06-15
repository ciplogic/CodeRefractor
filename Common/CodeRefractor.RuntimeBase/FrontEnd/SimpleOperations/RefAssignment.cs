#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class RefAssignment : LocalOperation
    {
        public LocalVariable Left;
        public LocalVariable Right;

        public RefAssignment()
            : base(OperationKind.RefAssignment)
        {
        }

        public override string ToString()
        {
            return string.Format("{0} = {1}", Left.Name, Right);
        }
    }
}