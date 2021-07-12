#region Uses

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class AlwaysBranch : LocalOperation
    {
        public AlwaysBranch()
            : base(OperationKind.AlwaysBranch)
        {
        }

        public int JumpTo { get; set; }

        public override string ToString()
        {
            return $"goto {JumpTo}:";
        }
    }
}