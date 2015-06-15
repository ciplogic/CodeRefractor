#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
{
    public class NewArrayObject : LocalOperation
    {
        public NewArrayObject()
            : base(OperationKind.NewArray)
        {
        }

        public LocalVariable AssignedTo { get; set; }
        public Type TypeArray { get; set; }
        public IdentifierValue ArrayLength { get; set; }

        public override string ToString()
        {
            return string.Format("{2} = new {0}[{1}]",
                TypeArray.FullName,
                ArrayLength,
                AssignedTo.Name);
        }
    }
}