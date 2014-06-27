#region Uses

using System;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.Identifiers
{
    public class SetArrayElement : LocalOperation
    {
        public IdentifierValue Index { get; set; }
        public LocalVariable Instance { get; set; }
        public IdentifierValue Right { get; set; }

        public SetArrayElement() : base(OperationKind.SetArrayItem)
        {
        }
        public Type GetElementType()
        {
            var computedType = Instance.ComputedType();
            var elementType = computedType.ClrType.GetElementType();
            return elementType;
        }

    }


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

    }
}