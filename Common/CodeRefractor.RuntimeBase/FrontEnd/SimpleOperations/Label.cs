#region Uses

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
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
            return $"label_{JumpTo}:";
        }
    }
}