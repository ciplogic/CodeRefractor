#region Uses

using System;
using CodeRefractor.Analyze;
using CodeRefractor.ClosureCompute;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Identifiers
{
    public class GetArrayElement : LocalOperation
    {
        public GetArrayElement()
            : base(OperationKind.GetArrayItem)
        {
        }

        public IdentifierValue Index { get; set; }
        public LocalVariable Instance { get; set; }
        public LocalVariable AssignedTo { get; set; }

        public Type GetElementType()
        {
            var computedType = Instance.ComputedType();
            var elementType = computedType.GetElementType();
            return elementType;
        }

        public override string ToString()
        {
            return $"{AssignedTo.Name}={Instance.Name}[{Index.Name}]";
        }

        public static GetArrayElement Create(LocalVariable assignedTo, LocalVariable instance, IdentifierValue index,
            ClosureEntities closureEntities)
        {
            var result = new GetArrayElement
            {
                AssignedTo = assignedTo,
                Instance = instance,
                Index = index
            };

            var elementType = instance.FixedType.GetClrType(closureEntities).GetElementType();
            assignedTo.FixedType = new TypeDescription(elementType);
            return result;
        }
    }
}