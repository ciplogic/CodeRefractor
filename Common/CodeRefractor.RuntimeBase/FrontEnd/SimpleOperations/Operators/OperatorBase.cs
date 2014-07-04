#region Uses

using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.Operators
{
    public class OperatorBase : LocalOperation
    {
        public LocalVariable AssignedTo { get; set; }

        public OperatorBase(string name, OperationKind kind) : base(kind)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}