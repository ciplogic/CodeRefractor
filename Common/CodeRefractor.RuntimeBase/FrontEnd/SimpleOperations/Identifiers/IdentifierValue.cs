#region Uses

using System;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.MiddleEnd.SimpleOperations.Identifiers
{
    public class IdentifierValue
    {
        public TypeDescription FixedType;

        public virtual TypeDescription ComputedType()
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