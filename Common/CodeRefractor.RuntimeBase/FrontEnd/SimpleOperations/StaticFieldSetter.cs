#region Uses

using System;
using CodeRefractor.FrontEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.FrontEnd.SimpleOperations
{
    public class StaticFieldSetter : LocalVariable
    {
        public Type DeclaringType;
        public string FieldName;

        public StaticFieldSetter()
        {
            Id = -1;
        }
    }
}