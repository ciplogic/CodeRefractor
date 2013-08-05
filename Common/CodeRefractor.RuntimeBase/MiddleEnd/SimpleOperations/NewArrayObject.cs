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
    }
}