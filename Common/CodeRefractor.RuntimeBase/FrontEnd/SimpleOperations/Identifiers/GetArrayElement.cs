#region Uses

using System;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

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

        public static GetArrayElement Create(LocalVariable assignedTo, LocalVariable instance, IdentifierValue index)
        {
            var result = new GetArrayElement
            {
                AssignedTo = assignedTo, 
                Instance = instance, 
                Index = index
            };

            var elementType = instance.FixedType.ClrType.GetElementType();
            assignedTo.FixedType = new TypeDescription(elementType);
            return result;
        }
    }
}