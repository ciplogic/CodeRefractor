#region Uses

using System;
using CodeRefractor.RuntimeBase.Analyze;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations.Identifiers
{
    public class IdentifierValue
    {
        public TypeDescription FixedType;

        public string Name => FormatVar();

        public virtual TypeDescription ComputedType()
        {
            if (FixedType == null && FormatVar() == "unknown")
                FixedType = new TypeDescription(null);
            return FixedType;
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