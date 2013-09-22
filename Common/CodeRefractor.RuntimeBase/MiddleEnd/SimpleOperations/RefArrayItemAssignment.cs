using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class RefArrayItemAssignment
    {
        public LocalVariable ArrayVar
        { get; set; }
        public IdentifierValue Index
        { get; set; }

        public LocalVariable Left { get; set; }

        public override string ToString()
        {
            return String.Format("{0} = & ({1}[{2}])", ArrayVar.Name, ArrayVar, Index);
        }
    }
}