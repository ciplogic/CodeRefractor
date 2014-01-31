using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class StaticFieldSetter : LocalVariable
    {
        public Type DeclaringType;
        public string FieldName;

        public StaticFieldSetter()
        {
            this.Id = -1;
        }
    }
}