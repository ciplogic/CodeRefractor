#region Uses

using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Operators
{
    public class OperatorBase : LocalOperation
    {
        public OperatorBase(string name, OperationKind kind) : base(kind)
        {
            Name = name;
        }

        public LocalVariable AssignedTo { get; set; }
        public string Name { get; set; }
    }
}