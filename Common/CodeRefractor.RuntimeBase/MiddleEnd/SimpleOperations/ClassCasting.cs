using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class ClassCasting
    {
        public IdentifierValue Value;
        public LocalVariable AssignedTo;

        public override string ToString()
        {
            return string.Format("{0} = ({1})( {2})",
                Value.Name,
                AssignedTo.FixedType,
                AssignedTo.Name);
        }
    }
}
