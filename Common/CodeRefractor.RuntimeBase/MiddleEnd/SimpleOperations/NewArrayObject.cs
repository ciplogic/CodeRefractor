#region Usings

using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class NewArrayObject : IdentifierValue
    {
        public Type TypeArray { get; set; }

        public IdentifierValue ArrayLength { get; set; }

        public override string ToString()
        {
            return string.Format("new {0}[{1}]", TypeArray.FullName, ArrayLength);
        }
    }
}