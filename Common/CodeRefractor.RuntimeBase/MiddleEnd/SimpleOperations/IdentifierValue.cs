using System;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
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
    }
}