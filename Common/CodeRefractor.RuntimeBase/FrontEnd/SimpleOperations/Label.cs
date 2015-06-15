#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class Label : LocalOperation
    {
        public Label()
            : base(OperationKind.Label)
        {
        }

        public int JumpTo { get; set; }

        public override string ToString()
        {
            return string.Format("label_{0}:", JumpTo);
        }
    }
}