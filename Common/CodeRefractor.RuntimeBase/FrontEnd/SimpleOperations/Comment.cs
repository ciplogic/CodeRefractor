using CodeRefractor.FrontEnd.SimpleOperations;

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