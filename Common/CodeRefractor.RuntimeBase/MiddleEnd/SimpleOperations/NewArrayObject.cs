#region Uses

using System;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations
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