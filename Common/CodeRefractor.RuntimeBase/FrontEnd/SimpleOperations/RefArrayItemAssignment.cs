#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;
using CodeRefractor.MiddleEnd.SimpleOperations;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class RefArrayItemAssignment : LocalOperation
    {
        public RefArrayItemAssignment()
            : base(OperationKind.AddressOfArrayItem)
        {
        }

        public LocalVariable ArrayVar { get; set; }
        public IdentifierValue Index { get; set; }

        public LocalVariable Left { get; set; }

        public override string ToString()
        {
            return String.Format("{0} = & ({1}[{2}])", ArrayVar.Name, ArrayVar, Index);
        }
    }
}