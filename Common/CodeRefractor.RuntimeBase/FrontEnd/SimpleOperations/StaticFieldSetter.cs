#region Uses

using System;
using CodeRefractor.MiddleEnd.SimpleOperations.Identifiers;

#endregion

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
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