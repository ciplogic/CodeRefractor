#region Uses

using System;

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
            var elementType = computedType.GetElementType();
            return elementType;
        }

        public override string ToString()
        {
            return string.Format("{0}[{1}]={2}", Instance.Name, Index.Name, Right.Name);
        }
    }
}