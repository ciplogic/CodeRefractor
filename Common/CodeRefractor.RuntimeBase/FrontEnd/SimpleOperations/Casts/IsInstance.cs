#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Casts
{
    public class IsInstance : LocalOperation
    {
        public IsInstance()
            : base(OperationKind.IsInstance)
        {
        }

        public LocalVariable AssignedTo { get; set; }
        public Type CastTo { get; set; }
        public IdentifierValue Right { get; set; }
    }
}