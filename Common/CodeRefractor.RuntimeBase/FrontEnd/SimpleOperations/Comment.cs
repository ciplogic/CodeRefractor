#region Uses

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class Comment : LocalOperation
    {
        public Comment()
            : base(OperationKind.Comment)
        {
        }

        public string Message { get; set; }
    }
}