#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.Identifiers
{
    public class SetArrayElement : LocalOperation
    {
        public SetArrayElement() : base(OperationKind.SetArrayItem)
        {
        }

        public IdentifierValue Index { get; set; }
        public LocalVariable Instance { get; set; }
        public IdentifierValue Right { get; set; }

        public Type GetElementType()
        {
            var computedType = Instance.ComputedType();
            var elementType = computedType.GetElementType();
            return elementType;
        }

        public override string ToString()
        {
            return $"{Instance.Name}[{Index.Name}]={Right.Name}";
        }
    }
}