#region Uses

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class Switch : LocalOperation
    {
        public Switch()
            : base(OperationKind.Switch)
        {
        }

        public int[] Jumps { get; set; }
    }
}