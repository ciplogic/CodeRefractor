using System;

namespace CodeRefractor.MiddleEnd.SimpleOperations.Identifiers
{
    public class GetArrayElement : LocalOperation
    {
        public IdentifierValue Index { get; set; }
        public LocalVariable Instance { get; set; }
        public LocalVariable AssignedTo { get; set; }

        public GetArrayElement()
            : base(OperationKind.GetArrayItem)
        {
        }
        public Type GetElementType()
        {
            var computedType = Instance.ComputedType();
            var elementType = computedType.ClrType.GetElementType();
            return elementType;
        }

        public override string ToString()
        {
            return string.Format("{0}={1}[{2}]", AssignedTo.Name, Instance.Name, Index.Name);
        }
    }
}