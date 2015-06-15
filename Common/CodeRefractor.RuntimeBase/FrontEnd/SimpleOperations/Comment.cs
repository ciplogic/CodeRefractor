#region Uses

using CodeRefractor.FrontEnd.SimpleOperations;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
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