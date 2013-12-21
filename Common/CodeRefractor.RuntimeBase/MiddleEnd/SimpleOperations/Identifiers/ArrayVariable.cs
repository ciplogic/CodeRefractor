#region Usings

using System;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers
{
    public class ArrayVariable : LocalVariable
    {
        public IdentifierValue Index { get; set; }
        public IdentifierValue Parent;

        public ArrayVariable(IdentifierValue parent, IdentifierValue id)
        {
            Id = -1;
            Kind = VariableKind.Local;
            Index = id;
            Parent = parent;
        }

        public Type GetElementType()
        {
            var computedType = ComputedType();
            var elementType = computedType.ClrType.GetElementType();
            return elementType;
        }

        public override TypeDescription ComputedType()
        {
            return Parent.ComputedType();
        }
    }
}