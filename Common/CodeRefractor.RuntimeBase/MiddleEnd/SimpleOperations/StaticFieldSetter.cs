using System;
using CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations.Identifiers;

namespace CodeRefractor.RuntimeBase.MiddleEnd.SimpleOperations
{
    public class StaticFieldSetter : LocalVariable
    {
        public IdentifierValue Instance;
        public Type DeclaringType;
        public string FieldName;
    }
}