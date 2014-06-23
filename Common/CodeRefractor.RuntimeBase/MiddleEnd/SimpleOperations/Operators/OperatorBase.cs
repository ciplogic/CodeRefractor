#region Usings

using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Operators
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