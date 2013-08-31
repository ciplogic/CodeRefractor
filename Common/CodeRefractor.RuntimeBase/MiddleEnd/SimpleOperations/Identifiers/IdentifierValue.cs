#region Usings

using System;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers
{
    public class IdentifierValue
    {
        public Type FixedType;

        public virtual Type ComputedType()
        {
            return FixedType;
        }

        public string Name
        {
            get { return FormatVar(); }
        }

        public virtual string FormatVar()
        {
            return "unknown";
        }

        public override string ToString()
        {
            return FormatVar();
        }

        public virtual IdentifierValue Clone()
        {
            throw new NotImplementedException();
        }
    }
}